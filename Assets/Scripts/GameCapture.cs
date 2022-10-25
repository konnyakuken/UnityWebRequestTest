using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

public class GameCapture : MonoBehaviour
{
    enum CaptureMode
    {
        GameView,
        inputCamera,
        inputTexture,
    }
    //ディレクトリのパス
    public static string directoryPath = "./ScreenShots";
    // ファイルの名前
    public string fileType = "ScreenShot";
    string fileName;
    // ファイルの生成番号名前
    public int nextGenerateCount = 0;
    //名前をつけて保存のパネルを開くかどうか
    [SerializeField]
    bool isOpenSaveFilePanel = false;
    // 何をCaptureするか変更する
    [SerializeField]
    CaptureMode selectCaptureMode;
    //出力中のファイルのパス
    private string generatedFilePath = null;
    // 出力するカメラ画像の指定
    [SerializeField]
    Camera inputCamera;
    // 出力するカメラ画像の指定
    [SerializeField]
    RenderTexture inputTexture;

    private void Awake()
    {
        SetPath();
    }

    //ゲームビューをキャプチャする
    public void ViewCapture()
    {
        //ファイルを出力中だったら
        if (generatedFilePath != null)
        {
            //ダイアログを表示
            if (EditorUtility.DisplayDialog("ファイルを出力中です", "出力中のファイル" + generatedFilePath + "は破棄されます。" + "\n" + "新しいファイルを保存しますか？", "保存", "中止"))
            {
                //出力中を解除
                generatedFilePath = null;
            }
            else
            {
                //処理を中止
                return;
            }
        }

        //ファイルのパス
        string filePath = null;
        // 保存先を指定して保存するかの条件分岐
        if (isOpenSaveFilePanel)
        {
            //ファイルの保存先を指定
            filePath = EditorUtility.SaveFilePanel("名前を付けて保存", "", fileName, "png");
            //パスの指定がなかったら処理を中止
            if (string.IsNullOrEmpty(filePath)) return;
        }
        else
        {
            //ファイルの保存先を作成
            filePath = string.Format(Path.Combine(directoryPath, fileName + ".png"));
            
            //フォルダが存在していなかったら
            if (!Directory.Exists(directoryPath))
            {
                //ダイアログを表示
                if (EditorUtility.DisplayDialog("フォルダが存在しません", "保存先のフォルダ" + directoryPath + "は存在しません。" + "\n" + "新しくフォルダを作成しますか？", "作成", "中止"))
                {
                    //フォルダを作成
                    Directory.CreateDirectory(directoryPath);
                    UnityEngine.Debug.Log("generated : " + directoryPath);
                }
                else
                {
                    //処理を中止
                    return;
                }

            }
        }

        if (selectCaptureMode == CaptureMode.GameView)
        {
            //ゲームビューをキャプチャ
            ScreenCapture.CaptureScreenshot(string.Format(filePath));
        }
        else if (selectCaptureMode == CaptureMode.inputCamera)
        {
            // inputCameraをキャプチャ
            var rt = new RenderTexture(inputCamera.pixelWidth, inputCamera.pixelHeight, 24);
            var prev = inputCamera.targetTexture;
            inputCamera.targetTexture = rt;
            inputCamera.Render();
            inputCamera.targetTexture = prev;
            RenderTexture.active = rt;

            var screenShot = new Texture2D(
                1920,
                1080,
                TextureFormat.RGB24,
                false);
            screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
            screenShot.Apply();

            var bytes = screenShot.EncodeToPNG();
            Destroy(screenShot);

            File.WriteAllBytes(filePath, bytes);
        }
        else if (selectCaptureMode == CaptureMode.inputTexture)
        {
            RenderTexture.active = inputTexture;
            var screenShot = new Texture2D(
                inputTexture.width,
                inputTexture.height,
                TextureFormat.RGB24,
                false);
            screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
            screenShot.Apply();

            var bytes = screenShot.EncodeToPNG();
            Destroy(screenShot);

            File.WriteAllBytes(filePath, bytes);
        }

        //キャプチャしたことを通知
        UnityEngine.Debug.Log("Captured : " + filePath);

        //EditorApplication.updateにコルーチンの進行を追加
        IEnumerator coroutine = GenerateFile(filePath);
        EditorApplication.update += () => coroutine.MoveNext();

    }

    public IEnumerator GenerateFile(string path)
    {

        //出力中に設定
        generatedFilePath = path;
        //出力されるまで待つ
        while (!File.Exists(path)) yield return null;
        // 今までに生成した枚数へ1追加して保存
        nextGenerateCount++;
        PlayerPrefs.SetInt("GenerateCount", nextGenerateCount);
        SetPath();
        //出力中を解除
        generatedFilePath = null;
        //出力されたことを通知
        UnityEngine.Debug.Log("generatedd : " + path);

        //保存した画像をサーバー側に送信
        PostPhoto.instance.PushPhoto(path);
    }

    /// <summary>
    /// Pathを設定する
    /// </summary>
    void SetPath()
    {
        nextGenerateCount = PlayerPrefs.GetInt("GenerateCount", 0);
        fileName = fileType + nextGenerateCount.ToString("0000");
    }

    public static implicit operator GameCapture(PostPhoto v)
    {
        throw new NotImplementedException();
    }
}