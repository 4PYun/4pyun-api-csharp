using System;
using System.Collections.Generic;
using System.Text;
using PYun.API;
using PYun.API.Reply;
using PYun.API.Request;
using PYun.Utils;

namespace api_demo
{
    class PYunServiceAPIImpl : AbstractServiceAPI
    {

        /// <summary>
        /// 查询停车费用
        /// 1001 订单获取成功，业务参数将返回。
        /// 1002 未查询到停车信息。
        /// 1003 月卡车辆，不允许缴费。
        /// 1500 接口处理异常。
        /// 
        /// // 获取订单代码
        /// if (card 不合法 && !plate 合法) {
        ///     return 1400; // 返回参数错误
        /// }
        /// // 1. 检查订单
        /// ParkingOrder parking = 获取停车记录(card, plate) // 根据车牌或停车卡获取停车记录
        /// if (parking 已出场) {
        ///     return 1002; // 无停车记录
        /// }
        /// // 2. 检查是否月卡、次卡等非临停用户
        /// if (parking 非临停) {
        ///     return 1003; // 非临停，返回无需支付停车费
        /// }
        /// // 3. 检查收费情况
        /// if (parking 无需交费) {
        ///     return parking; // 返回停车订单，不含支付单号，包含停车流水
        /// }
        /// // 4. 填充订单数据, 若有优惠，则填充优惠信息；
        /// ParkingBilling billing = 生成停车订单;
        /// parking.parking_order = billing.serial;    // 支付流水...
        /// parking.pay_value = billing.value;  // 实际支付金额
        /// 
        /// return parking; // 返回支付订单。
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override APIReply<ParkingBilling> ParkingBilling(ParkingBillingRequest request)
        {
            if (request.Plate == null)
            {
                return APIReply<ParkingBilling>.newBuilder()
                    .setResultCode(ResultCode.BadRequest)
                    .setMessage("车牌号码无效")
                    .Build();
            }
            // 
            ParkingBilling payload = new ParkingBilling();
            payload.EnterFreeTime = 15 * 60;    // 单位秒
            payload.BufferTime = 10 * 60;   // 单位秒
            payload.EnterTime = DateTime.Now;
            payload.Plate = "粤B660PP";
            payload.ParkingSerial = "12";
            payload.ParkingOrder = DateTime.Now.ToString("yyyyMMddHHmmss") + RandomUtil.RandomNumeric(6);
            payload.ParkingTime = 90 * 60;  // 单位秒
            payload.TotalValue = 1000;
            payload.FreeValue = 0;
            payload.PaidValue = 0;
            payload.PayValue = 1000;
         
            return APIReply<ParkingBilling>.newBuilder()
                .setResultCode(ResultCode.Success)
                .setPayload(payload)
                .setMessage("成功")
                .Build();
        }

        /// <summary>
        /// 同步场中支付结果
        /// 1001 接口处理成功，业务参数将返回。
        /// 1403 订单已撤销。
        /// 1500 接口内部处理失败。
        /// 
        /// // 1. 根据parking_order查询订单。
        /// ParkingPayment payment = 根据parking_order查询订单
        /// if (payment == null) {
        ///     return 1002;    // 未找到订单信息
        /// } elseif (payment 已成功处理) {
        ///     return 1001; // 返回处理成功
        /// }
        /// // 2. 根据parking_serial查询停车信息。
        /// ParkingOrder parking = 获取停车记录(parking_serial)；
        /// if (parking 已离场) {
        ///     retuen 1403; // 返回订单撤销，我们会根据这个状态核对订单，如果需要办理退款，我们会办理。
        /// }
        /// // 3. 检查订单是否为最新订单，我们需要保证用户支付的最新订单，才能保证支付金额足够
        /// if (payment 非本次停车最新订单) {
        ///     return 1403; // 返回订单撤销，我们会根据这个状态核对订单，如果需要办理退款，我们会办理。
        /// }
        /// // 4. 更新订单状态并更新总支付金额。
        /// update(parking);
        /// 
        /// return 1001; // 返回处理成功
        /// </summary>
        public override APIReply<object> ParkingPayment(ParkingPaymentRequest request)
        {
            if (request.ParkingSerial == null)
            {
                return APIReply<object>.newBuilder()
                    .setResultCode(ResultCode.BadRequest)
                    .setMessage("参数无效")
                    .Build();
            }


            return APIReply<object>.newBuilder()
                .setResultCode(ResultCode.Success)
                .Build();
        }

        /// <summary>
        /// 查询停车场实时信息
        /// 1001 获取成功，业务参数将返回。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingRealtime> ParkingRealtime(ParkingRealtimeRequest request)
        {
            return base.ParkingRealtime(request);
        }

        /// <summary>
        /// 同步停车记录
        /// 1001 获取成功，业务参数将返回。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingSYNCPayload> ParkingSYNC(ParkingSYNCRequest request)
        {
            return base.ParkingSYNC(request);
        }

        /// <summary>
        /// 更新自动支付状态
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车记录不存在
        /// 1403 车辆已出场。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingAutopayUpdate(ParkingAutopayUpdateRequest request)
        {
            return base.ParkingAutopayUpdate(request);
        }

        /// <summary>
        /// 更新锁车状态
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车记录不存在
        /// 1403 车辆已出场。
        /// 1500 接口处理异常。
        /// </summary>
        public virtual APIReply<object> ParkingLockingUpdate(ParkingLockingUpdateRequest request)
        {
            return base.ParkingLockingUpdate(request);
        }

        /// <summary>
        /// 无牌车直接入场请求
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 未检测到车辆
        /// 1403 短时间重复入场。
        /// 1500 接口处理异常。
        /// </summary>
        public virtual APIReply<ParkingDetail> ParkingDirectEnter(ParkingDirectEnterRequest request)
        {
            return base.ParkingDirectEnter(request);
        }

        /// <summary>
        /// 获取停车详情
        /// 1001 订单查询成功，业务参数将返回。
        /// 1002 未查询到停车信息。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingDetail> ParkingDetail(ParkingDetailRequest request)
        {
            ParkingPayment payment = new ParkingPayment();
            payment.PayType = PYun.API.Reply.ParkingPayment.PAY_TYPE_CASH;

            ParkingDetail detail = new ParkingDetail();
            detail.PlateColor = ColorType.Blue;
            return base.ParkingDetail(request);
        }

        /// <summary>
        /// 下发停车优惠
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车信息未找到。
        /// 1403 当前车辆已享受其他优惠。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingDiscountCreate(ParkingDiscountCreateRequest request)
        {
            return base.ParkingDiscountCreate(request);
        }

        /// <summary>
        /// 撤销停车优惠
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 派发信息未找到。
        /// 1403 优惠已使用。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingDiscountDestory(ParkingDiscountDestoryRequest request)
        {
            return base.ParkingDiscountDestory(request);
        }

        /// <summary>
        /// 查询月卡信息
        /// 1001 获取成功，业务参数将返回。
        /// 1002 没有查到相关贵宾记录。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingVIPPayload> ParkingVIPQuery(ParkingVIPQueryRequest request)
        {
            ParkingVIPPayload payload = new ParkingVIPPayload();
            List<ParkingVIP> Vips = new List<ParkingVIP>();
            ParkingVIP item = new ParkingVIP();
            item.Balance = 10;
            Vips.Add(item);

            payload.SetVips(Vips);

            return APIReply<ParkingVIPPayload>.newBuilder()
                .setPayload(payload)
                .setResultCode(ResultCode.Success)
                .setMessage("NOT IMPLEMENT YET")
                .Build();
        }

        /// <summary>
        /// 下发月卡续费信息
        /// 1001 续费成功，业务参数将返回。
        /// 1002 贵宾信息未找到。
        /// 1403 贵宾已被禁用，需退款。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingVIPRenewal(ParkingVIPRenewalRequest request)
        {
            return base.ParkingVIPRenewal(request);
        }

    }
}
