//using System;
//using System.IO;
//using System.Text.RegularExpressions;

//namespace CassetteHelper
//{
//    public interface ILineVisitor
//    {
//        bool Visit(TextReader input, Action<string> onMatch, Action<string> onNonMatch);
//    }

//    public class MatchingLineVisitor : ILineVisitor
//    {
//        private readonly IReplacementStrategy replacementStrategy;

//        public MatchingLineVisitor(IReplacementStrategy replacementStrategy)
//        {
//            this.replacementStrategy = replacementStrategy;
//        }

//        public bool Visit(TextReader input, Action<string> onMatch, Action<string> onNonMatch)
//        {
//            if (input == null) throw new ArgumentNullException("input");

//            string line;
//            bool containsMatches = false;

//            while ((line = input.ReadLine()) != null)
//            {
//                var replacement = replacementStrategy.Replace(line);
//                if (line == replacement)
//                {
//                    containsMatches = true;
//                    onMatch(line);
//                }
//                else
//                {
//                    onNonMatch(line);
//                }
//            }

//            return containsMatches;
//        }
//    }
//}