using System;
using System.Collections.Generic;
using System.Linq;

namespace Supersonic.BNF;

internal class Term
{
    #region Methods
    public void OrderByProperyIndexAndValidate(out string errorMessage)
    {
        Factors = Factors.OrderBy(x => x.PropertyIndex).ToList();

        //Make sure that all inequalities are at the end (can only have one)
        var success = CalcEqualityFactorsTerm(out _equalityFactors, out _inequalityFactors);
        if (!success)
        {
            errorMessage = "Only last Index Property in a Predicate can be an inequalitiy";
            return;
        }

        var currentPropIndex = -1;
        for (var i = 0; i < Factors.Count; i++)
        {
            if (Factors[i].PropertyIndex == currentPropIndex + 1)
            {
                currentPropIndex++;
            }
            else if (Factors[i].PropertyIndex == currentPropIndex)
            {
                //Do nothing;
            }
            else
            {
                errorMessage = $"Skiping index properties in Predicate is not allowed. Property #{i + 1} in index is skipped";
                return;
            }
        }

        errorMessage = null;
    }

    public bool CalcEqualityFactorsTerm(out List<Factor> equalityFactors, out List<Factor> inequalityFactors)
    {
        equalityFactors = new List<Factor>();
        inequalityFactors = new List<Factor>();

        var equalities = true;
        foreach (var factor in Factors)
        {
            if (factor.IsEquality && equalities)
            {
                equalityFactors.Add(factor);
            }
            else if (!factor.IsEquality)
            {
                if (inequalityFactors.Any(x => x.PropertyIndex != factor.PropertyIndex))
                {
                    equalityFactors = null;
                    inequalityFactors = null;
                    return false;
                }
                inequalityFactors.Add(factor);
                equalities = false;
            }
            else
            {
                equalityFactors = null;
                inequalityFactors = null;
                return false;
            }
        }
        return true;
    }

    public static List<Term> MultiplyTerms(List<Term> terms1, List<Term> terms2)
    {
        var newTerms = new List<Term>();
        foreach (var term in terms1)
        {
            foreach (var conditionTerm in terms2)
            {
                var newTerm = new Term();
                newTerm.Factors.AddRange(term.Factors);
                newTerm.Factors.AddRange(conditionTerm.Factors);
                newTerms.Add(newTerm);
            }
        }
        return newTerms;
    }
    #endregion

    #region Properties
    public List<Factor> Factors { get; set; } = new();

    public List<Factor> EqualityFactors
    {
        get
        {
            if (_equalityFactors == null)
            {
                var success = CalcEqualityFactorsTerm(out _equalityFactors, out _inequalityFactors);
                if (!success) throw new InvalidProgramException("Only last Index Properties in a Predicate can be an inequalities");
            }
            return _equalityFactors;
        }
    }
    private List<Factor> _equalityFactors;

    public List<Factor> InequalityFactors
    {
        get
        {
            if (_inequalityFactors == null)
            {
                var success = CalcEqualityFactorsTerm(out _equalityFactors, out _inequalityFactors);
                if (!success) throw new InvalidProgramException("Only last Index Properties in a Predicate can be an inequalities");
            }
            return _inequalityFactors;
        }
    }
    private List<Factor> _inequalityFactors;
    #endregion
}