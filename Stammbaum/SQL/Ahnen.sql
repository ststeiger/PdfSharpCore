
;WITH CTE AS 
( 
	SELECT 
		 person
		,ancestor
		,T_People.Gender 
		,T_People.FirstName
		,T_People.LastName
		,T_People.dtBirthDate 
		,T_People.PlaceOfBirth
		,T_People.Obit 
		,T_People.Education

		--,T_People.DeathSource 
		--,T_People.CauseOfDeath 
		--,T_People.PlaceOfCititzenship
		--,T_People.Religion
		 
		,1 AS generation 
		,tParentss.FirstName AS ParentsFirstName 
		,tParentss.LastName AS ParentsLastName 
		,ISNULL(tParentss.FirstName + ' ', '') + ISNULL(tParentss.LastName, '') AS ParentsName 
	FROM T_Ancestors
	LEFT JOIN T_People ON Id = T_Ancestors.person 
	LEFT JOIN T_People AS tParentss ON tParentss.Id = T_Ancestors.ancestor 

	WHERE T_People.LastName = 'Steiger' AND T_People.FirstName = 'Stephan'
	-- WHERE T_People.FirstName = 'Lara' AND T_People.LastName = 'Baumann' 
	-- WHERE T_People.LastName = 'Steiger' AND T_People.FirstName = 'Daniel'

	UNION ALL 


	SELECT 
		 T_People.id AS person
		,T_Ancestors.ancestor AS ancestor 
		,T_People.Gender 
		,T_People.FirstName
		,T_People.LastName
		,T_People.dtBirthDate 
		,T_People.PlaceOfBirth 
		,T_People.Obit 

		,T_People.Education
		--,T_People.DeathSource 
		--,T_People.CauseOfDeath 
		--,T_People.PlaceOfCititzenship
		--,T_People.Religion 
		 
		,CTE.generation + 1 AS generation 
		,tParentss.FirstName AS ParentsFirstName 
		,tParentss.LastName AS ParentsLastName 
		,ISNULL(tParentss.FirstName + ' ', '') + ISNULL(tParentss.LastName, '') AS ParentsName 
	FROM CTE 

	INNER JOIN T_People 
		ON T_People.Id = CTE.ancestor 

	INNER JOIN T_Ancestors 
		ON T_Ancestors.person = T_People.Id 

	INNER JOIN 
		(
			SELECT 
				 0 Id
				,0 AS Mother
				,0 AS Father
				,CAST(N'Unbekannt' AS nvarchar(255)) AS FirstName
				,CAST(N'Unbekannt' AS nvarchar(255)) AS LastName
				,CAST(N'Unbekannt' AS nvarchar(255)) AS FamilyName
				,dtBirthDate
				,BirthDate
				,BirthAdjunct
				,BirthDate_YearOnly
				,PlaceOfBirth
				,BirthSource
				,Obit
				,ObitAdjunct
				,PlaceOfObit
				,DeathSource
				,Baptism
				,BaptismAdjunct
				,PlaceOfBaptism
				,BaptismSource
				,BurialDate
				,BurialAdjunct
				,BurialPlace
				,BurialSource
				,RegisterNo
				,Profession
				,Comment
				,Street
				,ZIP
				,AddressLocation
				,Location
				,Country
				,Correspondence
				,Telephone
				,Gender
				,PlaceOfCititzenship
				,Religion
				,Education
				,Source
				,CauseOfDeath
				,New
				,Changed
			FROM Ahnen.dbo.T_People
			WHERE id = 1 

			UNION ALL 

			SELECT 
				 Id
				,Mother
				,Father
				,FirstName
				,LastName
				,FamilyName
				,dtBirthDate
				,BirthDate
				,BirthAdjunct
				,BirthDate_YearOnly
				,PlaceOfBirth
				,BirthSource
				,Obit
				,ObitAdjunct
				,PlaceOfObit
				,DeathSource
				,Baptism
				,BaptismAdjunct
				,PlaceOfBaptism
				,BaptismSource
				,BurialDate
				,BurialAdjunct
				,BurialPlace
				,BurialSource
				,RegisterNo
				,Profession
				,Comment
				,Street
				,ZIP
				,AddressLocation
				,Location
				,Country
				,Correspondence
				,Telephone
				,Gender
				,PlaceOfCititzenship
				,Religion
				,Education
				,Source
				,CauseOfDeath
				,New
				,Changed
			FROM T_People
	) AS tParentss 
		ON tParentss.Id = T_Ancestors.ancestor
)
SELECT 
	 person
	,ancestor
	,Gender
	,ISNULL(FirstName + ' ', '') + ISNULL(LastName, '') AS Name 
	,dtBirthDate
	,PlaceOfBirth
	,Obit
	,Education
	,generation
	,ParentsFirstName
	,ParentsLastName
	,CASE 
		WHEN ParentsName = 'Unbekannt Unbekannt' THEN 'Unbekannt'
		ELSE ParentsName 
	END AS ParentsName 
FROM CTE 

-- WHERE generation = 18
WHERE generation <= 6

ORDER BY generation, person, gender ASC 

-- SELECT POWER(2, 18)
