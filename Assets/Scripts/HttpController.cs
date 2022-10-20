using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;


public class HttpController : MonoBehaviour
{
    public InputField textField;
    public Text redirectText;


    public void PushButton(){
        //StartCoroutine(GetRequest());
        StartCoroutine(PostRequest());
    }

    private IEnumerator PostRequest()
    {
        //json形式に変換する
        Title sendMessage = new Title();

        sendMessage.title = textField.GetComponent<InputField>().text;
        string jsonstr = JsonUtility.ToJson(sendMessage);

        Debug.Log(jsonstr);

        //Postリクエストをする

        //POSTメソッドのリクエストを作成
        UnityWebRequest request = new UnityWebRequest("https://df26-160-247-196-229.jp.ngrok.io/photo", "POST");

        //json(string)をbyte[]に変換
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonstr);

        //jsonを設定
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        //ヘッダーにタイプを設定
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("リクエスト中");
                break;

            case UnityWebRequest.Result.Success:
                Debug.Log(request.downloadHandler.text);
                //受信したテキスト(json)を変換
                JsonClass jsonClass = JsonUtility.FromJson<JsonClass>(request.downloadHandler.text);
                redirectText.text = "LINEbotで「" + jsonClass.id.ToString() + "」を入力してね！";
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

    private IEnumerator GetRequest()
    {


        var request = UnityWebRequest.Get("https://df26-160-247-196-229.jp.ngrok.io/");

        yield return request.SendWebRequest();

        switch ( request.result )
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log( "リクエスト中" );
                break;

            case UnityWebRequest.Result.Success:
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


[System.Serializable]
public class  Title
{
    public string title;

}

//Jsonの変換用クラス
 [System.Serializable]
public class JsonClass
{

    public int id;
    public string title;

}