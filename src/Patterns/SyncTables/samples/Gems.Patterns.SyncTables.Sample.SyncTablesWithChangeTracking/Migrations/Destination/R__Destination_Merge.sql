CREATE OR REPLACE FUNCTION public.person_merge
(
    p_changed_data public.t_person_type[],
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
        DELETE FROM public.person
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
                FROM UNNEST(p_changed_data) AS cd
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
        INSERT INTO public.person
        (
            rec_id,
            person_id,
            first_name,
            last_name,
            age,
            gender
        )
        SELECT
            actual.rec_id,
            actual.person_id,
            actual.first_name,
            actual.last_name,
            actual.age,
            actual.gender
        FROM
        (
            SELECT
                ROW_NUMBER() OVER
                    (
                        PARTITION BY cd.rec_id
                        ORDER BY cd.ct_version DESC
                    ) AS rn,
                cd.rec_id,
                cd.person_id,
                cd.first_name,
                cd.last_name,
                cd.age,
                cd.gender,                
                cd.operation_type
            FROM UNNEST(p_changed_data) AS cd
        ) AS actual
        WHERE
            (actual.operation_type = 'I' OR actual.operation_type = 'U')
            AND rn = 1
        ON CONFLICT (rec_id) DO
        UPDATE SET
            rec_id           = EXCLUDED.rec_id,
            person_id        = EXCLUDED.person_id,
            first_name       = EXCLUDED.first_name,
            last_name        = EXCLUDED.last_name,
            age              = EXCLUDED.age,
            gender           = EXCLUDED.gender
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