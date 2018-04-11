
using System;
using System.Threading.Tasks;
using Flow;
using HyperMock;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.Interfaces;

namespace FlowTests
{
    [TestClass]
    public class FlowTests
    {


        [TestMethod]
        public async Task DoMeasureAndSendAsync_ExpectedDoMeasureException()
        {
            var errorText = "Forced exception!";

            var settingsReader = GetSettingsReader();

            var senseHatHandler = Mock.Create<ISenseHatHandler<SomeMeasure>>();
            senseHatHandler.Setup(x => x.GetDataFromSensorsAsync()).Throws(new DoMeasureException(errorText));


            var cloudHandler = Mock.Create<IAzureDeviceToCloudMessageHandler<SomeMeasure>>().Object;


            var logger = Mock.Create<ILogger<IFlow<SomeMeasure>>>().Object;


            var flow = new Flow<SomeMeasure>(senseHatHandler.Object, cloudHandler, logger, settingsReader);

            await Assert.ThrowsExceptionAsync<DoMeasureException>(async () => await flow.DoMeasureAndSendAsync());
        }


        [TestMethod]
        public async Task DoMeasureAndSendAsync_ExpectedSaveMeasureException()
        {
            Flow<SomeMeasure> flow = GetFlowWithCloudHandlerException();

            await Assert.ThrowsExceptionAsync<SaveMeasureException>(async () => await flow.DoMeasureAndSendAsync());
        }



        [TestMethod]
        public async Task RunFlowAsync_ExpectedSaveMeasureException()
        {

            Flow<SomeMeasure> flow = GetFlowWithCloudHandlerException();


            var tsk = flow.RunMeasurementContinuously();

            await Task.Delay(1000);

            Assert.IsInstanceOfType(flow.LoopException, typeof(SaveMeasureException));
        }

        [TestMethod]
        public void StopFlowRun_ExpectedCancelledCancellationTokenSource()
        {
            Flow<SomeMeasure> flow = GetFlowWithCloudHandlerException();

            flow.StopMeasurementRun();

            Assert.IsTrue(flow.CancellationTokenSource.IsCancellationRequested);
        }

        [TestMethod]
        public void Dispose_ExpectedNullCancellationTokenSource()
        {

            Flow<SomeMeasure> flow = GetFlowWithCloudHandlerException();
            flow.Dispose();

            Assert.IsNull(flow.CancellationTokenSource);

        }

        private static Flow<SomeMeasure> GetFlowWithCloudHandlerException()
        {

            var measure = new SomeMeasure() { Temp = 12 };

            var settingsReader = GetSettingsReader();


            var senseHatHandler = Mock.Create<ISenseHatHandler<SomeMeasure>>().Object;


            var cloudHandler = Mock.Create<IAzureDeviceToCloudMessageHandler<SomeMeasure>>();
            cloudHandler.Setup(z => z.HandleAsync(measure)).Throws<SaveMeasureException>();




            var logger = Mock.Create<ILogger<IFlow<SomeMeasure>>>().Object;

            var flow = new Flow<SomeMeasure>(senseHatHandler, cloudHandler.Object, logger, settingsReader);
            return flow;
        }

        private static ISettings GetSettingsReader()
        {
            var settings = Mock.Create<ISettings>();
            settings.Setup(x => x.NoOfMillisecondsForLoopInterval).Returns(100);
            return settings.Object;

        }
    }

    class SomeMeasure
    {
        public int Temp { get; set; }

    }

    class DoMeasureException : Exception
    {
        public DoMeasureException()
        {
        }

        public DoMeasureException(string message) : base(message)
        {
        }
    }

    class SaveMeasureException : Exception
    {
        public SaveMeasureException()
        {
        }

        public SaveMeasureException(string message) : base(message)
        {
        }
    }





}
