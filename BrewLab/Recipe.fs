namespace Models

open Units
open Calculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module Recipe = 
    type _recipe<[<Measure>] 'w, [<Measure>] 'v, [<Measure>] 't> = 
        { Name : string
          Grain : GrainAddition<'w> list
          Hops : hop<'w> list
          Adjuncts : adjunct<'w> list
          Yeast : yeast<'t> option
          Efficiency : float<percentage>
          BoilLength : float<minute>
          MashLength : float<minute>
          Volume : float<'v>
          Style : string 
          EstimatedOriginalGravity: float<sg>
          Bitterness:float<IBU>}
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    
    let UpdateGrain recipe grain = 
        { recipe with Grain = grain}

    let CalculateGravity volume efficiency grain =
        grain 
        |> List.fold (fun acc g -> acc + EstimateGravityPoints volume g.Weight g.Grain.Potential efficiency) 0.0<gp>
        |> ToGravity

    let EstimateOriginalGravity recipe = 
        {recipe with EstimatedOriginalGravity = recipe.Grain |> CalculateGravity recipe.Volume recipe.Efficiency}

    let AddGrain recipe grain = 
        grain :: recipe.Grain
        |> UpdateGrain recipe
        |> EstimateOriginalGravity