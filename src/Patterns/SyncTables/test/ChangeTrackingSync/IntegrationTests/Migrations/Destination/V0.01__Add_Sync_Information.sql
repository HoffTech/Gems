DROP TYPE IF EXISTS  public.t_change_tracking_info;
CREATE TYPE public.t_change_tracking_info as
(
    table_name              text,
    ct_version              bigint,
    update_time             timestamptz,
    last_restore_datetime   timestamptz
);

CREATE TABLE IF NOT EXISTS public.sync_info
(
    table_name              text            NOT NULL     PRIMARY KEY,
    ct_version              bigint          NOT NULL,
    update_time             timestamptz     NOT NULL,
    last_restore_datetime   timestamptz
);