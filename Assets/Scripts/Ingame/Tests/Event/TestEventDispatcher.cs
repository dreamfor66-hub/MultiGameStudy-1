using System.Text.RegularExpressions;
using NUnit.Framework;
using Rogue.Ingame.Event;
using UnityEngine.TestTools;
using UnityEngine;

namespace Tests
{
    public class EventTest : IEvent
    {

    }

    public class EventTest2 : IEvent
    {
    }

    public class EventTest3 : IEvent
    {

    }

    public class TestEventDispatcher
    {
        private int testValue = 0;

        private void OnEventTest(EventTest evt)
        {
            testValue++;
        }


        // 기본 Send / Listen.
        [Test]
        public void TestSend()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 1);
        }

        [Test]
        public void TestSendTwice()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 2);
        }



        [Test]
        public void TestListenZero()
        {
            testValue = 0;
            EventDispatcher.Send(new EventTest());
            Assert.True(testValue == 0);


        }

        [Test]
        public void TestListen()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 1);
        }

        [Test]
        public void TestRemove()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Remove<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest());
            Assert.True(testValue == 0);
        }

        [Test]
        public void TestNoListenOtherEvent()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest2());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 0);
        }

        [Test]
        public void TestListenTwice()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            LogAssert.Expect(LogType.Error, new Regex("already added handler"));
            Assert.True(testValue == 1);
        }


        [Test]
        public void TestListenDuringSendSameEvent()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTestListen);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTestListen);
            //Send 중간에 Listen 한거는 바로 추가되지 않는다.
            Assert.True(testValue == 0);

            //이타이밍에는 OnEventTest 가 추가되어있어야함.
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 1);
        }

        private void OnEventTestListen(EventTest evt)
        {
            EventDispatcher.Listen<EventTest>(OnEventTest);
        }

        /// <summary>
        /// Send 중에 같은 Listener Listen
        /// </summary>
        [Test]
        public void TestListenTwiceDuringSend()
        {
            EventDispatcher.Listen<EventTest>(OnEventListenTwiceDuringSend);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventListenTwiceDuringSend);
            LogAssert.Expect(LogType.Error, new Regex("already added handler"));
        }

        private void OnEventListenTwiceDuringSend(EventTest evt)
        {
            EventDispatcher.Listen<EventTest>(OnEventListenTwiceDuringSend);
        }


        /// <summary>
        /// Send 중에 Send 가 호출 될 경우,  
        /// </summary>
        [Test]
        public void TestSendDuringSendSameEvent()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTestSendSameEvent);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTestSendSameEvent);
            LogAssert.Expect(LogType.Error, new Regex("")); // 에러메시지가 나와야 한다.
        }

        private void OnEventTestSendSameEvent(EventTest evt)
        {
            testValue++;
            EventDispatcher.Send(new EventTest());
        }



        [Test]
        public void TestSendDuringSendOtherEvent()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTestSendOtherEvent);
            EventDispatcher.Listen<EventTest2>(OnEventTestSendOtherEvent2);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTestSendOtherEvent);
            EventDispatcher.Remove<EventTest2>(OnEventTestSendOtherEvent2);
            Assert.True(testValue == 2);
        }

        private void OnEventTestSendOtherEvent(EventTest evt)
        {
            testValue++;
            EventDispatcher.Send(new EventTest2());
        }

        public void OnEventTestSendOtherEvent2(EventTest2 evt)
        {
            testValue++;
        }

        [Test]
        public void TestSendChain()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTestSendChain1);
            EventDispatcher.Listen<EventTest2>(OnEventTestSendChain2);
            EventDispatcher.Listen<EventTest3>(OnEventTestSendChain3);
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Remove<EventTest>(OnEventTestSendChain1);
            EventDispatcher.Remove<EventTest2>(OnEventTestSendChain2);
            EventDispatcher.Remove<EventTest3>(OnEventTestSendChain3);
            LogAssert.Expect(LogType.Error, new Regex(""));
        }

        private void OnEventTestSendChain1(EventTest evt)
        {
            testValue++;
            EventDispatcher.Send(new EventTest2());
        }

        private void OnEventTestSendChain2(EventTest2 evt)
        {
            testValue++;
            EventDispatcher.Send(new EventTest3());
        }

        private void OnEventTestSendChain3(EventTest3 evt)
        {
            testValue++;
            EventDispatcher.Send(new EventTest());

        }

        [Test]
        public void TestSendRemoveSend()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTestSendListenSend);
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Listen<EventTest2>(OnEventTest2);

            EventDispatcher.Send(new EventTest());

            EventDispatcher.Remove<EventTest>(OnEventTest);
            Assert.True(testValue == 1);
        }

        private void OnEventTestSendListenSend(EventTest evt)
        {
            EventDispatcher.Remove<EventTest>(OnEventTestSendListenSend);


            //이전 구조에서는 여기에서 waitForAdd로 OnEventTest 가 Add 되면서 문제가 되었다.
            EventDispatcher.Send(new EventTest2());
        }

        private void OnEventTest2(EventTest2 evt)
        {
        }


        [Test]
        public void TestSilbling()
        {
            testValue = 0;
            EventDispatcher.Listen<EventTest>(OnEventTest);
            EventDispatcher.Listen<EventTest2>(OnEventTestSilbling);
            EventDispatcher.Send(new EventTest2());

            EventDispatcher.Remove<EventTest>(OnEventTest);
            EventDispatcher.Remove<EventTest2>(OnEventTestSilbling);

            Debug.Log(testValue);
            Assert.True(testValue == 2);
        }

        private void OnEventTestSilbling(EventTest2 evt)
        {
            EventDispatcher.Send(new EventTest());
            EventDispatcher.Send(new EventTest());
        }

    }
}


