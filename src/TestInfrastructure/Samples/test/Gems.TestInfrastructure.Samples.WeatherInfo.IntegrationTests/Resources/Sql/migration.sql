create table if not exists public.towns
(
    town_name varchar(50) not null
        constraint towns_pk
            primary key
);

alter table public.towns
    owner to postgres;

create table if not exists public.temperatures
(
    town_name varchar(50) not null
        constraint temperature_pk
            primary key,
    value     double precision
);

alter table public.temperatures
    owner to postgres;

create table if not exists public.precipitations
(
    town_name varchar(50) not null
        constraint precipitations_pk
            primary key,
    value     varchar(50)
);

alter table public.precipitations
    owner to postgres;

