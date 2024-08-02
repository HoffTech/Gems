
DROP PROCEDURE IF EXISTS public.sync_info_upsert_by_table_name;
CREATE OR REPLACE PROCEDURE public.sync_info_upsert_by_table_name(p_info public.t_change_tracking_info)
    LANGUAGE plpgsql
AS
$procedure$
BEGIN
    INSERT INTO public.sync_info
    (
        table_name,
        ct_version,
        update_time,
        last_restore_datetime
    )
    VALUES
    (
        p_info.table_name,
        p_info.ct_version,
        p_info.update_time,
        p_info.last_restore_datetime
    )
    ON CONFLICT (table_name) DO
    UPDATE SET
        ct_version                = EXCLUDED.ct_version,
        update_time               = EXCLUDED.update_time,
        last_restore_datetime     = EXCLUDED.last_restore_datetime
    WHERE sync_info.table_name    = EXCLUDED.table_name;
END;
$procedure$