CREATE OR REPLACE PROCEDURE public.destination_clear()
    LANGUAGE plpgsql
AS
$function$
BEGIN
    truncate table public.destination;
END;
$function$;