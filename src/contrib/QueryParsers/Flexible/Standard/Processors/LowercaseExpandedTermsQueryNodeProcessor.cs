﻿using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.QueryParsers.Flexible.Core.Processors;
using Lucene.Net.QueryParsers.Flexible.Core.Util;
using Lucene.Net.QueryParsers.Flexible.Standard.Config;
using Lucene.Net.QueryParsers.Flexible.Standard.Nodes;
using Lucene.Net.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Processors
{
    public class LowercaseExpandedTermsQueryNodeProcessor : QueryNodeProcessor
    {
        public LowercaseExpandedTermsQueryNodeProcessor()
        {
        }

        public override IQueryNode Process(IQueryNode queryTree)
        {
            bool? lowercaseExpandedTerms = QueryConfigHandler.Get(StandardQueryConfigHandler.ConfigurationKeys.LOWERCASE_EXPANDED_TERMS);

            if (lowercaseExpandedTerms != null && lowercaseExpandedTerms.Value)
            {
                return base.Process(queryTree);
            }

            return queryTree;
        }

        protected override IQueryNode PostProcessNode(IQueryNode node)
        {
            CultureInfo locale = QueryConfigHandler.Get(StandardQueryConfigHandler.ConfigurationKeys.LOCALE);
            if (locale == null)
            {
                locale = CultureInfo.CurrentCulture;
            }

            if (node is WildcardQueryNode
                || node is FuzzyQueryNode
                || (node is FieldQueryNode && node.Parent is IRangeQueryNode)
                || node is RegexpQueryNode)
            {

                ITextableQueryNode txtNode = (ITextableQueryNode)node;
                ICharSequence text = txtNode.Text;
                txtNode.Text = text != null ? UnescapedCharSequence.ToLowerCase(text, locale) : null;
            }

            return node;
        }

        protected override IQueryNode PreProcessNode(IQueryNode node)
        {
            return node;
        }

        protected override IList<IQueryNode> SetChildrenOrder(IList<IQueryNode> children)
        {
            return children;
        }
    }
}
