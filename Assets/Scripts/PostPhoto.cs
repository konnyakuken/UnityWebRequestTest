using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class  PostPhoto : MonoBehaviour
{
    public static PostPhoto instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public  void  PushPhoto( string filePath)
    {
        StartCoroutine(UploadFile(filePath));
    }

    IEnumerator UploadFile(string filePath)
    {

        // 画像ファイルをbyte配列に格納
        byte[] img = File.ReadAllBytes(filePath);

        // formにバイナリデータを追加
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", img,"image/png");
        // HTTPリクエストを送る
        UnityWebRequest request = UnityWebRequest.Post("https://16b1-27-127-169-37.jp.ngrok.io/files/", form);

        //yield return request.Send();
        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("リクエスト中");
                break;

            case UnityWebRequest.Result.Success:
                // POSTに成功した場合，レスポンスを出力
                Debug.Log(request.downloadHandler.text);
                break;

            case UnityWebRequest.Result.ConnectionError:
                Debug.Log
                (
                    @"サーバとの通信に失敗。
                    リクエストが接続できなかった、
                    セキュリティで保護されたチャネルを確立できなかったなど。"
                );
                break;

            case UnityWebRequest.Result.ProtocolError:
                Debug.Log
                (
                    @"サーバがエラー応答を返した。
                    サーバとの通信には成功したが、
                    接続プロトコルで定義されているエラーを受け取った。"
                );
                break;

            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log
                (
                    @"データの処理中にエラーが発生。
                    リクエストはサーバとの通信に成功したが、
                    受信したデータの処理中にエラーが発生。
                    データが破損しているか、正しい形式ではないなど。"
                );
                break;


        }
    }
}
