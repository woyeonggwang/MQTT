using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System;
using System.Collections;

public class MyMqttClient : M2MqttUnityClient
{
    private bool isTryingToReconnect = false;
    private int reconnectDelaySeconds = 5; // 재시도 간격 (초)
    private float timer;
    protected override void Awake()
    {
        // Start() 전에 연결 설정을 먼저 초기화
        autoConnect = true; // ✅ 자동 연결 ON
    }

    protected override void Start()
    {
        Debug.Log("👀 Start 실행됨");

        brokerAddress = "mqtt broker address";
        brokerPort = 1883;
        isEncrypted = false;

        mqttUserName = "mqtt user name";
        mqttPassword = "mqtt password";

        base.Start();
    }

    public void StartMqttOn()
    {
        Connect();
    }
    public void DestroyMqttOff()
    {
        Disconnect();
    }
    protected override void OnConnected()
    {
        Debug.Log("✅ MQTT 연결 성공!");

        client.Subscribe(
            new string[] { "timecapsule/new" },
            new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE }
        );

        //client.ConnectionClosed += (s, e) =>
        //{
        //    Debug.Log("연결 끊김, 재연결 시도 중...");
        //    TryReconnect();
        //};
        // 메시지 수신 핸들러 연결
        client.MqttMsgPublishReceived += HandleMessageReceived;
    }
    void TryReconnect()
    {
        //if (!isTryingToReconnect) // 중복 실행 방지
        //{

        //    isTryingToReconnect = true;

        //    while (!client.IsConnected)
        //    {
        //        Debug.Log("MQTT 재연결 시도 중...");

        //        try
        //        {
        //            string clientId = Guid.NewGuid().ToString();
        //            client.Connect(clientId, mqttUserName, mqttPassword);
        //            if (client.IsConnected)
        //            {
        //                Debug.Log("MQTT 재연결 성공!");
        //                // 필요한 경우 다시 Subscribe
        //                client.Subscribe(new string[] { "my/topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        //                break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.LogWarning($"MQTT 재연결 실패: {ex.Message}");
        //        }

        //        //yield return new WaitForSeconds(reconnectDelaySeconds);
        //    }

        //    isTryingToReconnect = false;
        //}
    }
    private void HandleMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string topic = e.Topic;
        string message = Encoding.UTF8.GetString(e.Message);
        Debug.Log($"📩 MQTT 메시지 수신됨\nTopic: {topic}\nMessage: {message}");
        ProjectManager.instance.jsonStr = message;
        ProjectManager.instance.dataCount = 0;
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.LogError("❌ MQTT 연결 실패: " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        Debug.Log("🔌 MQTT 연결 끊김");
    }

    private void OnDestroy()
    {
        Disconnect();
        Debug.Log("MQTT 연결 종료");
    }
}