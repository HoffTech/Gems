CREATE TABLE IF NOT EXISTS public.destination
(
    rec_id                  bigint          NOT NULL     PRIMARY KEY,
    item_id                 varchar(20)     NOT NULL,
    text_data               varchar(20)     NOT NULL,
    numeric_data            numeric(32, 16) NOT NULL
);

DROP TYPE IF EXISTS  public.t_destination_entity;
CREATE TYPE public.t_destination_entity as
(
    ct_version              bigint,
    operation_type          char(1),
    
    rec_id                  bigint,
    item_id                 varchar(20),
    text_data               varchar(20),
    numeric_data            numeric(32, 16)
);

DROP TYPE IF EXISTS public.t_destination_merge_result;
CREATE TYPE public.t_destination_merge_result AS
(
    inserted_count         int,
    updated_count          int,
    deleted_count          int
);