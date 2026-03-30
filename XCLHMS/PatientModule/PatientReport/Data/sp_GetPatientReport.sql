ALTER PROCEDURE sp_GetPatientReport
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @PatientType NVARCHAR(50) = NULL,
    @ChallanNo NVARCHAR(50) = NULL,
    @MRNo NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id AS PatientID,
        p.MRNo AS MRNo,
        p.Name AS PatientName,
        p.FName AS FatherName,
        p.Age,
        p.Gender,
        p.NIC,
        p.NICType,
        p.ContactNo AS Mobile,
        p.Address,
        p.BloodGroup,
        ISNULL(pt.Name, 'N/A') AS PatientType,
        '' AS DoctorName, 
        COALESCE(p.AdmitDate, p.CreatedDate) AS VisitDate,
        ISNULL(c.IRCode, '') AS ChallanNo,
        p.Description
    FROM Pateint p 
    LEFT JOIN PatientType pt ON p.PatientTypeID = pt.Id
    LEFT JOIN IR c ON c.customerId = p.Id
    WHERE 
        (@ChallanNo IS NULL OR @ChallanNo = '' OR c.IRCode LIKE '%' + @ChallanNo + '%')
        AND (@MRNo IS NULL OR @MRNo = '' OR p.MRNo LIKE '%' + @MRNo + '%')
        AND (
            @StartDate IS NULL 
            OR CAST(COALESCE(p.AdmitDate, p.CreatedDate) AS DATE) >= CAST(@StartDate AS DATE)
        )
        AND (
            @EndDate IS NULL 
            OR CAST(COALESCE(p.AdmitDate, p.CreatedDate) AS DATE) <= CAST(@EndDate AS DATE)
        )
        AND (
            @PatientType IS NULL OR @PatientType = '' 
            OR pt.Name LIKE '%' + @PatientType + '%'
            OR (@PatientType = 'InPatient' AND (pt.Name LIKE '%In%' OR pt.Name LIKE '%Indoor%'))
            OR (@PatientType = 'OutPatient' AND (pt.Name LIKE '%Out%' OR pt.Name LIKE '%Outdoor%'))
        )
    ORDER BY COALESCE(p.AdmitDate, p.CreatedDate) DESC;
END
GO


