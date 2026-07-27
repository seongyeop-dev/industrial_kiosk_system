using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yeop;

//Log 1개 데이터
//JSON 저장과 CSV Export 공용 모델
[Serializable]
public class LogData
{
    public string timeText;
    public string category;
    public string title;
    public string message;
    public string summary;
    public string detail;
}

//JSON 저장용 래퍼 클래스
//리스트 직렬화를 안정적으로 처리하기 위한 구조
[Serializable]
public class LogSaveData
{
    public List<LogData> logs = new List<LogData>();
}

//Log 파일 저장/불러오기/Export 전용 클래스
//Application.persistentDataPath 기준
//JSON 저장
//CSV Export
public static class scr_LogFileSave
{
    private const string ROOT_FOLDER = "KioskData";
    private const string LOG_FOLDER_NAME = "Logs";
    private const string LOG_JSON_FILE_NAME = "system_log.json";

    //로그 폴더 경로 반환
    public static string GetLogFolderPath()
    {
        return Path.Combine(
            Application.persistentDataPath, ROOT_FOLDER, LOG_FOLDER_NAME);
    }

    //기본 JSON 저장 경로 반환
    public static string GetLogJsonPath()
    {
        return Path.Combine(GetLogFolderPath(), LOG_JSON_FILE_NAME);
    }

    //CSV Export 경로 반환
    //Export 할 때마다 시간 포함 파일명 생성
    public static string GetNewCsvExportPath()
    {
        string fileName = "system_log_export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        return Path.Combine(GetLogFolderPath(), fileName);
    }

    //전체 로그 JSON 저장
    public static void SaveLogs(List<LogData> logs)
    {
        LogSaveData saveData = new LogSaveData();

        if (logs != null)
        {
            saveData.logs = new List<LogData>(logs);
        }

        string jsonPath = GetLogJsonPath();
        NST_Json.ExportJson(saveData, jsonPath);

        Debug.Log("[scr_LogFileSave] Log JSON Saved : " + jsonPath);
    }

    //JSON 파일에서 전체 로그 불러오기
    public static List<LogData> LoadLogs()
    {
        string jsonPath = GetLogJsonPath();

        if (!File.Exists(jsonPath))
        {
            Debug.Log("[scr_LogFileSave] Log JSON not found : " + jsonPath);
            return new List<LogData>();
        }

        LogSaveData saveData = NST_Json.ImportJson<LogSaveData>(jsonPath);

        if (saveData == null || saveData.logs == null)
        {
            Debug.LogWarning("[scr_LogFileSave] Loaded Log JSON is empty or invalid.");
            return new List<LogData>();
        }

        Debug.Log("[scr_LogFileSave] Log JSON Loaded : " + jsonPath);
        return saveData.logs;
    }

    //전체 로그를 CSV 파일로 Export
    public static void ExportLogsToCsv(List<LogData> logs)
    {
        if (logs == null || logs.Count == 0)
        {
            Debug.LogWarning("[scr_LogFileSave] No logs to export.");
            return;
        }

        List<Dictionary<string, object>> csvRows = new List<Dictionary<string, object>>();

        for (int i = 0; i < logs.Count; i++)
        {
            LogData log = logs[i];

            Dictionary<string, object> row = new Dictionary<string, object>();
            row["Time"] = log.timeText;
            row["Category"] = log.category;
            row["Title"] = log.title;
            row["Message"] = log.message;
            row["Summary"] = log.summary;
            row["Detail"] = log.detail;

            csvRows.Add(row);
        }

        string csvPath = GetNewCsvExportPath();
        NST_CSV.ExportCSV(csvRows, csvPath);

        Debug.Log("[scr_LogFileSave] Log CSV Exported : " + csvPath);
    }
}