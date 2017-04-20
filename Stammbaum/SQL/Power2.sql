
;WITH POWER_2 AS 
(
	SELECT 0 AS e
	UNION ALL 
	SELECT e + 1 AS e 
	FROM POWER_2 WHERE e < 30
)
SELECT 
	 '2^' + CAST(e AS varchar(20)) + ' = ' + CAST(POWER(2, e) AS varchar(20)) AS numAncestors 
FROM POWER_2 

-- SELECT  2^30 = 1'073'741'824 -- 30*35 = 1050
