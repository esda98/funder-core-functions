using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Data
{
    static class Queries
    {
        public const string GET_TEST_STRINGS = "SELECT test.fx_get_test_strings();";
        public const string ADD_FUNDRAISER = "SELECT core.fx_add_fundraiser(@id, @acctId, @name, @start, @end);";
        public const string GET_FUNDRAISER = "SELECT core.fx_get_fundraiser(@acctId)";
        public const string GET_ITEM = "SELECT core.fx_get_item(@acctId)";
        public const string ADD_ITEM = "SELECT core.fx_add_item(@id, @acctId, @name, @price, @quantity);";
        public const string GET_ITEMS_FUNDRAISERS = "SELECT core.fx_get_items_fundraisers(@acctId, @itemId)";
        public const string GET_FUNDRAISER_ITEMS = "SELECT core.fx_get_fundraiser_items(@acctId, @fundId)";
    }
}
