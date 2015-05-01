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
          EstimatedOriginalGravity: float<sg>}
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    
    let UpdateGrain recipe grain = 
        { recipe with Grain = grain}

    let EstimateOriginalGravity recipe = 
        {recipe with EstimatedOriginalGravity = recipe.Grain 
                            |> List.fold (fun acc g -> acc + EstimateGravity recipe.Volume g.Weight g.Grain.Potential recipe.Efficiency) 0.0<sg>} 
    
//    let EstimateOriginalGravity recipe = 
//        match recipe with
//        | Metric mr -> 
//            {mr with EstimatedOriginalGravity = mr.Grain 
//                            |> List.fold (fun acc g -> acc + EstimateGravity mr.Volume g.Weight g.Potential mr.Efficiency) 0.0<sg>}
//        | Imperial ir -> 
//            {ir with EstimatedOriginalGravity = ir.Grain 
//                    |> List.fold (fun acc g -> acc + EstimateGravity (ir.Volume |> ToLitres) (g.Weight |> ToKilograms) (g.Potential |> ToPGPKg) ir.Efficiency) 0.0<sg>}
    
    let AddGrain recipe grain = 
        grain :: recipe.Grain
        |> UpdateGrain recipe
        |> EstimateOriginalGravity