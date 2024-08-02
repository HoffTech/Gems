CREATE OR REPLACE FUNCTION public.destination_merge
(
    changed_data public.t_destination_entity[],
    enable_full_changes_log bool
)
    RETURNS public.t_destination_merge_result
    LANGUAGE plpgsql
AS
$function$
DECLARE
    result public.t_destination_merge_result;

    deleted_count   int;
    inserted_count  int;
    updated_count   int;
BEGIN

    WITH deleted AS
    (
        DELETE FROM public.destination
        WHERE rec_id = any
        (
            SELECT last_changes.rec_id
            FROM
            (
                SELECT
                    ROW_NUMBER() OVER
                        (
                            PARTITION BY cd.rec_id
                            ORDER BY cd.ct_version DESC
                        ) AS rn,
                    cd.operation_type,
                    cd.rec_id
                FROM UNNEST(changed_data) AS cd
            ) AS last_changes
            WHERE
                last_changes.operation_type = 'D'
                AND rn = 1
        )
        RETURNING 1
    )
    SELECT COUNT(*) FROM deleted INTO deleted_count;

    WITH upserted AS
    (
        INSERT INTO public.destination
        (
            rec_id,
            item_id,
            text_data,
            numeric_data
        )
        SELECT
            actual.rec_id,
            actual.item_id,
            actual.text_data,
            actual.numeric_data
        FROM
        (
            SELECT
                ROW_NUMBER() OVER
                    (
                        PARTITION BY cd.rec_id
                        ORDER BY cd.ct_version DESC
                    ) AS rn,
                cd.rec_id,
                cd.item_id,
                cd.text_data,
                cd.numeric_data,
                cd.operation_type
            FROM UNNEST(changed_data) AS cd
        ) AS actual
        WHERE
            (actual.operation_type = 'I' OR actual.operation_type = 'U')
            AND rn = 1
        ON CONFLICT (rec_id) DO
        UPDATE SET
            item_id          = EXCLUDED.item_id,
            text_data        = EXCLUDED.text_data,
            numeric_data     = EXCLUDED.numeric_data
        RETURNING XMAX
    ),
    counters AS
    (
        SELECT
            SUM(CASE WHEN XMAX = 0 THEN 1 ELSE 0 END) inserted,
            SUM(CASE WHEN XMAX::text::bigint > 0 THEN 1 ELSE 0 END) updated
        FROM upserted
    )
    SELECT inserted, updated FROM counters INTO inserted_count, updated_count;

    SELECT
        COALESCE(inserted_count, 0),
        COALESCE(updated_count, 0),
        COALESCE(deleted_count, 0)
    INTO result;

RETURN result;
END;
$function$;