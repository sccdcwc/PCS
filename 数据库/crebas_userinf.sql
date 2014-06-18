drop table JC_YH;

drop table TBPZ;

drop table YH_BDML;

drop table YH_BDWJ;

drop table YH_DLPZ;

drop table YH_PZ;

create table JC_YH (
YH_GUID              VARCHAR(12)                    not null,
YHDLM                VARCHAR(32),
DLMM                 VARCHAR(128),
ZWXM                 VARCHAR(64),
YHLX                 VARCHAR(8),
primary key (YH_GUID)
);

create table TBPZ (
TBConfigID           INTEGER                        not null,
TBTime               DATE,
TBServerID           VARCHAR(32),
primary key (TBConfigID)
);

create table YH_BDML (
YH_BDML_ID           INTEGER                        not null,
YH_GUID              VARCHAR(12),
FJML_ID              INTEGER,
SFYZML               VARCHAR(1),
MLBM                 VARCHAR(8),
MLBTMC               VARCHAR(64),
CJSJ                 DATE,
GXSJ                 DATE,
PXH                  INTEGER,
BZ                   VARCHAR(256),
primary key (YH_BDML_ID),
foreign key (YH_GUID)
      references JC_YH (YH_GUID)
);

create table YH_BDWJ (
YH_BDWJ_ID           INTEGER                        not null,
YH_BDML_ID           INTEGER,
YH_GUID              VARCHAR(12),
WJLX                 VARCHAR(1),
ZYLY                 VARCHAR(1),
JMFS                 VARCHAR(1),
WJMD5                CHAR(10),
WJURL                CHAR(10),
WJMC                 CHAR(10),
FORMAT_LIST          CHAR(10),
"SIZE"               CHAR(10),
SUBJECT_CODE         CHAR(10),
GRADE_CODE           CHAR(10),
VERSION              CHAR(10),
USAGE_TYPE_CODE      INTEGER,
GXSJ                 DATE,
primary key (YH_BDWJ_ID),
foreign key (YH_BDML_ID)
      references YH_BDML (YH_BDML_ID),
foreign key (YH_GUID)
      references JC_YH (YH_GUID)
);

create table YH_DLPZ (
DLConfigID           INTEGER                        not null,
RemPass              CHAR(1),
AutoLog              VARCHAR(1),
primary key (DLConfigID)
);

create table YH_PZ (
YH_GUID              VARCHAR(12),
TBConfigID           INTEGER,
DLConfigID           INTEGER,
foreign key (YH_GUID)
      references JC_YH (YH_GUID),
foreign key (TBConfigID)
      references TBPZ (TBConfigID),
foreign key (DLConfigID)
      references YH_DLPZ (DLConfigID)
);

