﻿using ImcFramework.Core.Distribution;
using ImcFramework.WcfInterface;
using ImcFramework.WcfInterface.TransferMessage;
using System.Diagnostics;

namespace ImcFramework.Core
{
    public class DefaultMutilUserProgress : IMutilUserProgress
    {
        private EServiceType serviceType;
        private IDistributionFacility<ProgressInfoMessage> distributionFacility;

        public DefaultMutilUserProgress(EServiceType serviceType)
        {
            this.serviceType = serviceType;
            distributionFacility = DistributionFacilityFactory.GetDistributionFacility<ProgressInfoMessage>();
        }

        //设置总数
        public void SendTaskProgressTotal(int count, TotalType totalType)
        {
            var sf = new StackFrame();
            var mn = sf.GetMethod().Name;

            var msgEntity = ProgressInfoMessageBuilder.Create()
                        .WithCallbackMethodName(mn)
                        .WithServiceType(serviceType)
                        .WithTotal(count)
                        .WithTotalType(totalType)
                        .Build();

            distributionFacility.Push(msgEntity);
        }

        //设置单个账号的总数
        public void SendTaskProgressItemTotal(string user, int total)
        {
            var sf = new StackFrame();
            var mn = sf.GetMethod().Name;

            ProgressInfoMessage msgEntity = ProgressInfoMessageBuilder.Create()
                .WithCallbackMethodName(mn)
                .WithServiceType(serviceType)
                .WithTotal(total)
                .WithUser(user)
                .Build();

            distributionFacility.Push(msgEntity);
        }

        //设置单个账号+1
        public void SendTaskProgressIncrease(string user)
        {
            var sf = new StackFrame();
            var mn = sf.GetMethod().Name;

            ProgressInfoMessage msgEntity = ProgressInfoMessageBuilder.Create()
                .WithCallbackMethodName(mn)
                .WithServiceType(serviceType)
                .WithUser(user)
                .WithValue(1)
                .Build();

            distributionFacility.Push(msgEntity);
        }

        //设置单个账号强制完成
        public void ForceFinish(string user)
        {
            var sf = new StackFrame();
            var mn = sf.GetMethod().Name;
            ProgressInfoMessage msgEntity = ProgressInfoMessageBuilder.Create()
                       .WithCallbackMethodName(mn)
                       .WithServiceType(serviceType)
                       .WithUser(user)
                       .Build();

            distributionFacility.Push(msgEntity);
        }

        //结束
        public void FinishAll()
        {
            var sf = new StackFrame();
            var mn = sf.GetMethod().Name;

            ProgressInfoMessage msgEntity = ProgressInfoMessageBuilder.Create()
                .WithCallbackMethodName(mn)
                .WithServiceType(serviceType)
                .Build();

            distributionFacility.Push(msgEntity);
        }
    }
}
