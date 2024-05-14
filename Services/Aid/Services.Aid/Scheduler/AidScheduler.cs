using System.Threading;
using System;
using MongoDB.Driver;
using Services.Aid.Services;

namespace Services.Aid.Scheduler
{
    public class AidScheduler
    {
        private Timer _timer;
        private readonly int _intervalInMilliseconds = 24 * 60 * 60 * 1000; // 24 saat
        private readonly IHumaneAidService _humaneAidService;
        private readonly IBasisAidService _basisAidService;

        public AidScheduler(IBasisAidService basisAidService,IHumaneAidService humaneAidService)
        {
            _basisAidService = basisAidService;
            _humaneAidService = humaneAidService;
        }
        public void Start()
        {
            // İlk kontrolü başlatmak için biraz bekleyin
            var dueTime = CalculateDueTime();
            //var dueTime = TimeSpan.Zero;


            _timer = new Timer(CheckAndDeleteOldData, null, dueTime, TimeSpan.FromMilliseconds(_intervalInMilliseconds));
        }

        private void CheckAndDeleteOldData(object state)
        {
            _basisAidService.DeleteOldData();
            _humaneAidService.DeleteOldData();
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private TimeSpan CalculateDueTime()
        {
            var now = DateTime.Now;
            var tomorrow = now.Date.AddDays(1); // Yarının başlangıcı
            var dueTime = tomorrow - now;
            return dueTime;
        }
    }
}
