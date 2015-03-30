namespace Models

open Units
open Caculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module Recipe = 
    type _recipe<[<Measure>] 'w, [<Measure>] 'v, [<Measure>] 't> = 
        { Name : string
          Grain : grain<'w> list
          Hops : hop<'w> list
          Adjuncts : adjunct<'w> list
          Yeast : yeast<'t>
          Efficiency : float<percentage>
          BoilLength : float //In minutes
          MashLength : float //In minutes
          Volume : float<'v>
          Style : string }
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    
    let EstimatedOriginalGravity recipe = 
        match recipe with
        | Metric mr -> 
            mr.Grain
            |> List.map (fun g -> (g.Weight |> ToPound, g.Potential |> ToPGP))
            |> EstimateGravityFromGrainBill (mr.Volume |> ToUsGallons) mr.Efficiency
        | Imperial ir -> 
            ir.Grain 
            |> List.map (fun g -> (g.Weight, g.Potential))
            |> EstimateGravityFromGrainBill ir.Volume ir.Efficiency