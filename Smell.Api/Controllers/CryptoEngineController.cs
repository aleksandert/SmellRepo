using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Smell.Api.Model;
using Smell.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smell.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoEngineController : ControllerBase
    {
        private readonly ILogger<CryptoEngineController> _logger;
        private readonly Lib.MetaExchangeEngine _engine;

        public CryptoEngineController(ILogger<CryptoEngineController> logger, Smell.Lib.MetaExchangeEngine engine)
        {
            _logger = logger;
            _engine = engine;
        }

        /// <summary>
        /// Send order request for processing.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Result with list of trades.</returns>
        [HttpPost("ProcessOrder")]
        public TradesResponse ProcessOrder(OrderRequestModel model)
        {
            if (ModelState.IsValid) {

                var request = new OrderRequest()
                {
                    Type = model.Type,
                    Amount = model.Amount
                };

                return _engine.ProcessOrder(request);
            }
            return null;
        }

        /// <summary>
        /// Returns crypto exchange data.
        /// </summary>
        [HttpGet("GetCryptoExchanges")]
        public IEnumerable<CryptoExchangeModel> GetCryptoExchanges()
        {
            var model = _engine.GetCryptoExchanges()
                            .Select(x => new CryptoExchangeModel()
                            {
                                Name = x.Name,
                                Asks = x.OrderBook.Asks.Count(),
                                Bids = x.OrderBook.Bids.Count(),
                                BalanceBTC = x.Balance.BTC,
                                BalanceEUR = x.Balance.EUR,
                            });

            return model;
        }
    }
}
