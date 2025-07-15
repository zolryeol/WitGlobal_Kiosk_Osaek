using System.Collections.Generic;

public class WITMissionVO
{
    public int Code { get; set; } // Code (public)
    public string Msg { get; set; } // Message (public)
    
    public List<ResponseData> Data { get; set; } // List of Response Data (public)
}

public class ResponseData
{
    public int PK { get; set; } // Primary Key (public)
    public string PST_CN { get; set; } // Post Content (public)
    public int LK_CNT { get; set; } // Like Count (public)
    public string CRT_DT { get; set; } // Creation Date (public)
    public int CMNT_CNT { get; set; } // Comment Count (public)
    public int NST_CMNT_CNT { get; set; } // Non-System Comment Count (public)
    public List<CTS_JOIN_MBR_TB> CTS_JOIN_MBR_TB { get; set; } // Join Member Data (public)
    public MBR MBR { get; set; } // Member Data (public)
    public List<PST_LK_JOIN_MBR_TB> PST_LK_JOIN_MBR_TB { get; set; } // Like Join Member Data (public)
    public List<FILE_TB> FILE_TB { get; set; } // File Data (public)
    public object PST_ANS_TB { get; set; } // Post Answer (public)
    public MSN_TB MSN_TB { get; set; } // Mission Data (public)
}

public class MSN_TB
{
    public int PK { get; set; } // Primary Key (public)
    public int MSN_ATND_CNT { get; set; } // Attendance Count (public)
    public string MSN_BGNG_DT { get; set; } // Mission Start Date (public)
    public string MSN_END_DT { get; set; } // Mission End Date (public)
    public List<MSN_JOIN_MBR_TB> MSN_JOIN_MBR_TB { get; set; } // Join Member Data (public)
    public PRIZE_TB PRIZE_TB { get; set; } // Prize Data (public)
    public MSN_TY_QA_TB MSN_TY_QA_TB { get; set; } // QA Data (public)
    public object MSN_TY_MCQA_TB { get; set; } // MCQA Data (public)
    public object MSN_TY_IMG_TB { get; set; } // Image Data (public)
}

public class MSN_TY_QA_TB
{
    public string MSN_QSTN { get; set; } // Question (public)
    public string ANS { get; set; } // Answer (public)
}

public class FILE_TB
{
    public string FILE_PATH { get; set; } // File Path (public)
    public string FILE_TY_CD { get; set; } // File Type Code (public)
    public string FILE_NM { get; set; }
}

public class PRIZE_TB
{
    public string PRIZE_NM { get; set; } // Prize Name (public)
    public string TXT_PRIZE { get; set; } // Prize Text (public)
    public List<FILE_TB> FILE_TB { get; set; } // File Data (public)
}

public class MSN_JOIN_MBR_TB
{
    public int PK { get; set; } // Primary Key (public)
}

public class CTS_JOIN_MBR_TB
{
    public int PK { get; set; } // Primary Key (public)
}

public class MBR
{
    public int PK { get; set; } // Primary Key (public)
    public string NC_NM { get; set; } // Nickname (public)
    public List<FILE_TB> FILE_TB { get; set; } // File Data (public)
}

public class PST_LK_JOIN_MBR_TB
{
    public int PK { get; set; } // Primary Key (public)
}
