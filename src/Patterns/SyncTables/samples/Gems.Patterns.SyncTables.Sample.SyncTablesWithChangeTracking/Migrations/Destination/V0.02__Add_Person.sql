CREATE TABLE IF NOT EXISTS public.person
(
    rec_id                  bigint          NOT NULL     PRIMARY KEY,
    person_id               uuid            NOT NULL,
    first_name              varchar(20)     NOT NULL,
    last_name               varchar(20)     NOT NULL,
    age                     int,
    gender                  int
);

DROP TYPE IF EXISTS  public.t_person_type;
CREATE TYPE public.t_person_type as
(
    ct_version              bigint,
    operation_type          char(1),

    rec_id                  bigint,
    person_id               uuid,
    first_name              varchar(20),
    last_name               varchar(20),
    age                     int,
    gender                  int
);

DROP TYPE IF EXISTS public.t_destination_merge_result;
CREATE TYPE public.t_destination_merge_result AS
(
    inserted_count         int,
    updated_count          int,
    deleted_count          int
);