CREATE OR REPLACE PROCEDURE public.person_clear()
    LANGUAGE plpgsql
AS
$function$
BEGIN
    truncate table public.person;
END;
$function$;