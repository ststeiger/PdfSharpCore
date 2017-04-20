 
;WITH CTE AS 
(
	SELECT 
		 T_People.Id
		,T_People.gender 
		,T_People.Mother
		,T_People.Father
		,CAST(NULL AS bigint) AS Child 
		,CAST(NULL AS char(1)) AS ChildGender 
		,CAST(NULL AS national character varying(1000)) AS ChildName 
		,T_People.FirstName
		,T_People.LastName
		,T_People.FamilyName
		,T_People.dtBirthDate 
		,T_People.PlaceOfBirth
		
		
		,
		CAST
		(
			CASE T_People.gender 
				WHEN 'M' THEN ISNULL(NULLIF(T_People.FirstName, '') + ' ', '') + ISNULL(T_People.LastName, '') 
				ELSE 
					  ISNULL(NULLIF(T_People.FirstName, '') + ' ', '') 
					+ ISNULL(T_People.FamilyName, '') 
					+ ISNULL(' - ' + NULLIF(T_People.LastName, ''), '') 
			END 
			AS national character varying(1000)
		)
		AS composite_name 
		
		,0 AS generation 
	FROM T_People
	WHERE T_People.LastName = 'Steiger' AND T_People.FirstName = 'Stephan' 
	
	UNION ALL 
	
	SELECT 
		 tParentss.Id
		,tParentss.gender 
		,tParentss.Mother
		,tParentss.Father
		,CTE.Id AS Child 
		,CTE.gender AS ChildGender  
		,CAST(CTE.composite_name AS national character varying(1000)) AS ChildName 
		,tParentss.FirstName
		,tParentss.LastName
		,tParentss.FamilyName
		,tParentss.dtBirthDate 
		,tParentss.PlaceOfBirth
		
		,
		CAST
		(
			CASE 
				WHEN tParentss.Id = 0 THEN 'Unbekannt' 
				WHEN tParentss.gender = 'M' THEN ISNULL(NULLIF(tParentss.FirstName, '') + ' ', '') + ISNULL(tParentss.LastName, '') 
				ELSE 
					  ISNULL(NULLIF(tParentss.FirstName, '') + ' ', '') 
					+ ISNULL(tParentss.FamilyName, '') 
					+ ISNULL(' - ' + NULLIF(tParentss.LastName, ''), '') 
			END 
			AS national character varying(1000)
		) AS composite_name 
		
		,CTE.generation + 1 AS generation  
	FROM CTE 
	
	INNER JOIN T_Ancestors  
		ON T_Ancestors.Person = CTE.Id 
		
	INNER JOIN 
		(
			SELECT 
				 0 Id
				,0 AS Mother
				,0 AS Father
				,CAST(N'Unbekannt' AS national character varying(255)) AS FirstName
				,CAST(N'Unbekannt' AS national character varying(255)) AS LastName
				,CAST(N'Unbekannt' AS national character varying(255)) AS FamilyName
				,CAST(NULL as datetime2) AS dtBirthDate
				,BirthDate
				,BirthAdjunct
				,BirthDate_YearOnly
				,CAST(N'Unbekannt' AS national character varying(255)) AS PlaceOfBirth
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

SELECT * FROM CTE 
ORDER BY generation, child, Id, gender 
