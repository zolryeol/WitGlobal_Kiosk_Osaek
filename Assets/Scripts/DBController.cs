using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DBController : MonoBehaviour
{
    public static MySqlConnection sqlConn;

    static string ipAddress = "134.185.113.244";
    static string db_id = "root";
    //static string db_pw = "1234";
    static string db_pw = "1q2w3e4r5t";
    static string db_name = "kiosk";

    string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8mb4;SslMode=none;", ipAddress, db_id, db_pw, db_name);


    private void Awake()
    {
        try
        {
            sqlConn = new MySqlConnection(strConn);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }


    //select ��ȸ ������ ���
    //2��° �Ķ���� table_name�� Dataset �̸��� �����ϱ� ����
    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        try
        {
            sqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = sqlConn;
            cmd.CommandText = p_query;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, table_name);

            sqlConn.Close();  //DB ���� ����

            return ds;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }


    private void OnApplicationQuit()
    {
        sqlConn.Close();
    }
}
