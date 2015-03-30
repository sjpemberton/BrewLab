namespace Models

open Units
open Caculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module Recipe =

    type Recipe = {
        Name: string
        Grain: grain list
        Hops: hop list
        Adjuncts: adjunct list
        Yeast: yeast
        Efficiency: float<percentage>
        BoilLength:float//In minutes
        MashLength:float//In minutes
        Volume:float<L>
    }

    let EstimatedOriginalGravity recipe = 
        recipe.Grain
        |> List.map (fun g -> {Weight = (g.Weight / 1000.0<g> * 1.0<kg>) |> ToPound; Potential = g.Potential |> ToPGP})
        |> EstimateGravityFromGrainBill (recipe.Volume |> ToUsGallons) recipe.Efficiency