SELECT TS.USER_NAME, 
       TS.FULL_NAME, 
       TS.TEAM, 
       TS.START_DATE,
       TS.END_DATE,
       TS.WEEK_NUMBER,
       TS.STATE 
FROM   (SELECT TM.USER_NAME, 
               TM.FULL_NAME, 
               TM.CREATE_DATE, 
               TM.START_DATE, 
               TM.END_DATE, 
               (SELECT Stuff((SELECT DISTINCT ',' + RP.RESOURCEPOOL_NAME 
								FROM   USERACCOUNT U 
                                     JOIN RESOURCEPOOLASSIGNMENT RPA ON RPA.RESOURCE_ID = U.USERACCOUNT_ID 
                                     JOIN RESOURCEPOOL RP ON RP.RESOURCEPOOL_ID = RPA.RESOURCEPOOL_ID 
								WHERE 1=1
								AND U.USER_NAME = TM.USER_NAME 
                                AND RP.LOCKED = 0 
                                AND RPA.ACTIVE = 1 
								FOR xml path('')), 1, 1, '')) TEAM, 
               CASE 
                 WHEN Count(TM.STATE) = 0 THEN 'Unsubmitted' 
                 ELSE TM.STATE 
               END                                          STATE, 
               TM.WEEK_NUMBER, 
               TM.CALENDARWEEK_ID 
        FROM   (SELECT USR.USER_NAME, 
                       USR.FULL_NAME, 
                       CONVERT(DATE, USR.CREATE_DATE)           CREATE_DATE, 
                       C.CALENDARWEEK_ID, 
                       C.START_DATE, 
                       C.END_DATE, 
                       C.WEEK_NUMBER, 
                       (SELECT CASE TSM.FLOW_STATE 
                                 WHEN '1' THEN 'Waiting Approve' 
                                 WHEN '2' THEN 'Approved' 
                                 WHEN '3' THEN 'Rejected' 
                                 WHEN '4' THEN 'Re-Opened' 
                                 ELSE 'Unsubmitted' 
                               END AS FLOW_STATE 
                        FROM   TIMESHEETHEADER TH 
                               JOIN TIMESHEETFLOWMASTER TSM ON TSM.TIMESHEETHEADER_ID = TH.TIMESHEETHEADER_ID 
                               JOIN USERACCOUNT U ON U.USERACCOUNT_ID = TH.RESOURCE_ID 
							   WHERE  1 = 1 
                               AND TH.CALENDARWEEK_ID = C.CALENDARWEEK_ID 
                               AND U.USER_NAME = USR.USER_NAME) AS STATE 
                FROM   CALENDARWEEK C, 
                       USERACCOUNT USR 
                WHERE   1 = 1 
                        AND C.YEAR = Year(Getdate()) 
                        AND ( C.WEEK_NUMBER >= 1 
                        AND C.WEEK_NUMBER <= Datepart(wk, Getdate()) - 1) 
                        AND USR.USER_NAME IN (SELECT DISTINCT U.USER_NAME 
                                             FROM   RESOURCEPOOLASSIGNMENT RPA 
                                                    JOIN RESOURCEPOOL RP ON RP.RESOURCEPOOL_ID = RPA.RESOURCEPOOL_ID 
                                                    JOIN USERACCOUNT U ON U.USERACCOUNT_ID = RPA.RESOURCE_ID 
                                             WHERE  U.LOCKED = 0 
                                                    AND U.USER_NAME NOT IN ('PPMIntegrationUser', 
                                                        'master', 'admin', 'infoera', 'levent.ozalp', 'hakan.turgut', 'yahya.ozturk', 'canturk.toprakli', 'alphan.arslan', 'cengizhan.gunaydin' ) 
                                                    AND RP.RESOURCEPOOL_NAME NOT IN ('Interns')) 
													AND USR.LOCKED = 0) TM 
        GROUP  BY TM.USER_NAME, TM.FULL_NAME, TM.CREATE_DATE, TM.START_DATE, TM.END_DATE, STATE, TM.CALENDARWEEK_ID, TM.WEEK_NUMBER) TS 
WHERE  TS.STATE = 'Unsubmitted' 
AND TS.END_DATE > CREATE_DATE
--CUSTOM CRITERIAS
AND (TS.USER_NAME != 'samed.solak' OR (TS.WEEK_NUMBER NOT IN (6, 7) OR YEAR(TS.START_DATE) != 2018))
AND (TS.USER_NAME != 'berkay.yildiz' OR (TS.WEEK_NUMBER NOT IN (1, 2) OR YEAR(TS.START_DATE) != 2018))
AND (TS.USER_NAME != 'selin.erdogan' OR ((TS.WEEK_NUMBER NOT IN (26, 27, 28, 29, 30, 31, 45, 46) AND MONTH(TS.START_DATE) NOT IN (8,9,10)) OR YEAR(TS.START_DATE) != 2018))
AND (TS.WEEK_NUMBER NOT IN (34) OR YEAR(TS.START_DATE) != 2018)
--CUSTOM CRITERIAS
ORDER  BY TS.USER_NAME, TS.WEEK_NUMBER