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
        public const string EDIT_FUNDRAISER = "SELECT core.fx_edit_fundraiser(@fund)";
        public const string EDIT_ITEM = "SELECT core.fx_edit_item(@item)";
        public const string SET_FUNDRAISER_ITEMS = "SELECT core.fx_set_fundraiser_items(@fundId, @itemIds)";
        public const string SET_ITEM_FUNDRAISERS = "SELECT core.fx_set_item_fundraisers(@itemId, @fundIds)";
        public const string GET_SELLERS = "SELECT core.fx_get_sellers(@acctId)";
        public const string ADD_WITHDRAWAL = "SELECT core.fx_add_withdrawal(@id, @itemId, @sellerId, @price, @time, @handler, @quantity);";
        public const string GET_WITHDRAWALS = "SELECT core.fx_get_withdrawals(@acctId)";
        public const string ADD_PAYMENT = "SELECT core.fx_add_payment(@id, @wiId, @payType, @amt, @time, @handler);";
        public const string DELETE_ITEM = "SELECT core.fx_delete_item(@itemId)";
    }
}
