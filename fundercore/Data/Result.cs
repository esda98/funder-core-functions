using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Data {
    class Result {
        public bool status;
        public string message;
        public Result(bool givenStatus, string givenMessage) {
            status = givenStatus;
            message = givenMessage;
        }
    }
}
