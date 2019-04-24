using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Data {
    class Result {
        public bool success;
        public string message;
        public Result(bool givenStatus, string givenMessage) {
            success = givenStatus;
            message = givenMessage;
        }
    }
}
