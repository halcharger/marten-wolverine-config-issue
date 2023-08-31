# marten-wolverine-config-issue

When configuring Wolverine along with Marten DDL scripts appear to no longer be generated.

## Steps to reproduce issue

- Build solution
- Run Api project
- Execute `utils/db-schema-ddl` endpoint
- Notice DDL statements for User document like:

```
DROP TABLE IF EXISTS public.mt_doc_user CASCADE;
CREATE TABLE public.mt_doc_user (
   id                  varchar                     NOT NULL,
   data                jsonb                       NOT NULL,
   mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
   mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
   mt_dotnet_type      varchar                     NULL,
   joined_utc          varchar(50)                 NOT NULL,
CONSTRAINT pkey_mt_doc_user_id PRIMARY KEY (id)
);

CREATE UNIQUE INDEX mt_doc_user_uidx_email ON public.mt_doc_user USING btree ((data ->> 'Email'));

CREATE INDEX mt_doc_user_idx_rate_payers_association_ref_id ON public.mt_doc_user USING btree ((data -> 'RatePayersAssociationRef' ->> 'Id'));

CREATE INDEX mt_doc_user_idx_rate_payers_association_ref_municipality_ref_id ON public.mt_doc_user USING btree ((data -> 'RatePayersAssociationRef' -> 'MunicipalityRef' ->> 'Id'));

CREATE INDEX mt_doc_user_idx_joined_utc ON public.mt_doc_user USING btree (joined_utc);
```

- This is the happy path, it is working as expected.

Now, uncomment lines 35-38 in `Program.cs`

- Build solution
- Run Api project
- Execute `utils/db-schema-ddl` endpoint
- Notice complete **ABSENCE** of User document DDL statements!!!
