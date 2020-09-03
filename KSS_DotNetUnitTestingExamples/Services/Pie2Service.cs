using Serilog;
using Services.Dto;
using Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Services.Helpers.LoggingHelper;
using static Services.Helpers.StatusCodesHttp;

namespace Services
{
    /// <summary>
    /// Improved Pie service 
    /// </summary>
    public interface IPie2Service
    {
        Task<PieResponse> GetPieRecord(string flavour);
    }

    public class Pie2Service : IPie2Service
    {
        private IPie2DataService _pieDataService;
        private IPastryService _pastryService;
        private IFillingService _fillingService;
        private ILogger _logger;
        private INowAdapter _nowService;

        private readonly string[] RecognisedFlavours = { "Cherry", "Apple", "Cheese" };

        public Pie2Service(
            IPie2DataService pieDataService,
            IPastryService pastryService,
            IFillingService fillingService,
            ILogger logger,
            INowAdapter nowService)
        {
            _pieDataService = pieDataService;
            _pastryService = pastryService;
            _fillingService = fillingService;
            _logger = logger;
            _nowService = nowService;
        }

        public async Task<PieResponse> GetPieRecord(string flavour)
        {
            var response = new PieResponse
            {
                StatusCodeHttp = Ok
            };
            flavour = GetFormattedFlavour(flavour);
            if(!IsRecognisedFlavour(flavour))
            {
                _logger.Warning("Flavour {Flavour} not allowed. Allowed flavours {RecognisedFlavours}. {CodeInfo}", flavour, RecognisedFlavours, GetCodeInfo());
                response.StatusCodeHttp = BadRequest;
                return response;
            }
            int pastryRequired = 50;
            int pastryReceived = GetPastry(pastryRequired);
            if (pastryReceived != pastryRequired)
            {
                _logger.Error("Pastry required {Required}. Pastry received {Received}. {CodeInfo}", pastryRequired, pastryReceived, GetCodeInfo());
                response.StatusCodeHttp = InternalServerError;
                return response;
            }
            int fillingRequired = 50;
            int fillingReceived = GetFilling(flavour, fillingRequired);
            if (fillingReceived != fillingRequired)
            {
                _logger.Error("{Flavour} filling required {Required}. Filling received {Received}. {CodeInfo}", flavour, fillingRequired, fillingReceived, GetCodeInfo());
                response.StatusCodeHttp = InternalServerError;
                return response;
            }

            Pie pie = await _pieDataService.BakePie(flavour, pastryReceived, fillingReceived, _nowService.Now());
            if(pie == null)
            {
                _logger.Error("{Flavour} Pie not made. {CodeInfo}", flavour, GetCodeInfo());
                response.StatusCodeHttp = InternalServerError;
                return response;
            }
            List<Pie> pieAudit = await _pieDataService.GetPieAudit();
            if(pieAudit == null || pieAudit.Count == 0)
            {
                _logger.Error("No pie audit. {CodeInfo}", GetCodeInfo());
            }

            return new PieResponse
            {
                PieRecord = new PieRecord
                {
                    MostRecent = pie,
                    PieAudit = pieAudit
                },
                StatusCodeHttp = Ok
            };
        }


        private string GetFormattedFlavour(string flavour)
        {
            return flavour?.Trim().ToLowerInvariant();
        }

        private Boolean IsRecognisedFlavour(string flavour)
        {
            return RecognisedFlavours.Contains(flavour, StringComparer.InvariantCultureIgnoreCase);
        }

        private int GetPastry(int quantity)
        {
            bool orderRequired;
            try
            {
                return _pastryService.Get(quantity);
            }
            catch (ArgumentException)
            {
                orderRequired = true;
            }
            if (orderRequired)
            {
                _logger.Information("Ordering pastry. {CodeInfo}", GetCodeInfo());
                _pastryService.Order();
                try
                {
                    return _pastryService.Get(quantity);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Unable to get {Quantity} of pastry. {CodeInfo}", quantity, GetCodeInfo());
                    return 0;
                }
            }
            return 0;
        }

        private int GetFilling(string flavour, int quantity)
        {
            bool orderRequired;
            try
            {
                return _fillingService.Get(flavour, quantity);
            }
            catch (ArgumentException)
            {
                orderRequired = true;
            }
            if (orderRequired)
            {
                _logger.Information("Ordering {Flavour} filling. {CodeInfo}", flavour, GetCodeInfo());
                _fillingService.Order(flavour);
                try
                {
                    return _fillingService.Get(flavour, quantity);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Unable to get {Quantity} of filling. {CodeInfo}", quantity, GetCodeInfo());
                    return 0;
                }
            }
            return 0;
        }
    }
}