DROP FUNCTION IF EXISTS public.sync_info_get_last_for_table;
CREATE OR REPLACE FUNCTION public.sync_info_get_last_for_table(p_table_name text)
    RETURNS public.t_change_tracking_info
    LANGUAGE plpgsql
AS
$function$
DECLARE
    sync_info public.t_change_tracking_info;
BEGIN
    SELECT
        table_name,
        ct_version,
        update_time,
        last_restore_datetime
    FROM public.sync_info
    WHERE table_name = p_table_name
    INTO sync_info;

    RETURN sync_info;
END;
$function$;
