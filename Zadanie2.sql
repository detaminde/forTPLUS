WITH RankedEntries AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY group_id ORDER BY dt DESC) AS rn
    FROM test1
)
SELECT id, dt, group_id
FROM RankedEntries
ORDER BY group_id, rn DESC;