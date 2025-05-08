using System;
using Photon.Bolt;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Client, "BoltBase")]
    public class BoltNetworkChecker : GlobalEventListener
    {
        private float pingPeriod = 1f;
        private float pingTime = 0f;

        private int sendCount;
        private int receiveCount;
        private int ping;

        private void FixedUpdate()
        {
            if (pingTime <= 0f)
            {
                pingTime = pingPeriod;
                Ping();
            }
            pingTime -= Time.fixedDeltaTime;
        }

        private void OnGUI()
        {
            var str = $"ping : {ping}. send rate : {sendCount} / {receiveCount}. (unreliable event)";
            GUI.color = Color.green;
            GUI.Label(new Rect(300, 0, 300, 20), str);
            GUI.color = Color.white;
        }

        private void Ping()
        {
            sendCount++;
            var ping = PingEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.Unreliable);
            ping.time = Time.realtimeSinceStartup;
            ping.Send();

        }

        public override void OnEvent(PongEvent evnt)
        {
            receiveCount++;
            ping = (int)((Time.realtimeSinceStartup - evnt.time) * 1000);
        }
    }
}
