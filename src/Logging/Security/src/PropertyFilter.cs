// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gems.Logging.Security
{
    internal class PropertyFilter<TElement> : IPropertyFilter<TElement> where TElement : class
    {
        private readonly IPropertyVisitor<TElement> visitor;

        public PropertyFilter(ISecureKeyProvider secureKeyProvider, IPropertyVisitor<TElement> visitor)
        {
            this.SecureKeyProvider = secureKeyProvider;
            this.visitor = visitor;
        }

        public ISecureKeyProvider SecureKeyProvider { get; }

        public void Filter(TElement root)
        {
            var actions = new List<Tuple<SecureKey, IPropertyProxy>>();
            this.visitor.Visit(root, p =>
            {
                var key = this.SecureKeyProvider.Keys().FirstOrDefault(x => x.Matcher.IsMatch(p));
                if (key != null)
                {
                    actions.Add(new Tuple<SecureKey, IPropertyProxy>(key, p));
                }
            });

            actions.ForEach(a =>
            {
                var key = a.Item1;
                var prop = a.Item2;
                switch (key.Action)
                {
                    case SecureKeyAction.Update:
                        if (key.ValuePattern != null)
                        {
                            prop.Update(key.ValuePattern.Replace(prop.Value?.ToString() ?? string.Empty, key.ReplaceText));
                        }

                        break;
                    case SecureKeyAction.Remove:
                        prop.Remove();
                        break;
                }
            });
        }
    }
}
