﻿using ImcFramework.Infrastructure;
using ImcFramework.WcfInterface;
using ImcFramework.WcfInterface.TransferMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImcFramework.Core.MqModuleExtension
{
    public class MqModule : ModuleExtensionBase
    {
        private IDistributionFacility<MessageEntity> m_MqDistribution;
        private List<Type> m_TypeList;

        public MqModule(ServiceContext serviceContext)
            : base(serviceContext)
        {
            m_MqDistribution = DistributionFacilityFactory.GetDistributionFacility<MessageEntity>();

            m_TypeList = new List<Type>();
        }

        private readonly string MQ_MODULE = "MQ_MODULE";

        public void RegisterType(Type type)
        {
            if (!m_TypeList.Contains(type))
            {
                m_TypeList.Add(type);
            }
        }

        public void UnRegisterType(Type type)
        {
            m_TypeList.Remove(type);
        }

        public override string Name
        {
            get
            {
                return MQ_MODULE;
            }
        }

        public override void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (Defaults.IsIsolatedJob)
                {
                    System.Threading.Thread.Sleep(1);

                    var msgs = m_MqDistribution.ReadMessages() as IEnumerable<MessageEntity>;

                    foreach (var msg in msgs.OrderBy(zw => zw.Timestamp))
                    {
                        if (!msg.IsProgressMsg)
                        {
                            Observers.BroadCastMessageWithMQ(msg);
                        }
                        else
                        {
                            try
                            {
                                switch (msg.CallbackMethodName)
                                {
                                    case "SendTaskProgressTotal":
                                        Observers.SendTaskProgressTotal(msg.ServiceType, msg.Total, msg.TotalType, true);
                                        break;
                                    case "SendTaskProgressItemTotal":
                                        Observers.SendTaskProgressItemTotal(msg.ServiceType, msg.SellerAccount, msg.Total, true);
                                        break;
                                    case "SendTaskProgressIncrease":
                                        Observers.SendTaskProgressIncrease(msg.ServiceType, msg.SellerAccount, msg.Value, true);
                                        break;
                                    case "SendTaskProgressForceFinish":
                                        Observers.SendTaskProgressForceFinish(msg.ServiceType, msg.SellerAccount, true);
                                        break;
                                    case "SendTaskProgressFinishAll":
                                        Observers.SendTaskProgressFinishAll(msg.ServiceType, true);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("我去，出大事了:" + ex.Message + ex.StackTrace);
                            }
                        }
                    }
                }
            });
        }

        public override void Stop()
        {
            base.Stop();
        }
    }
}
