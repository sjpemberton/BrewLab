namespace Models

open Units
open Calculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module Recipe = 
    type _recipe<[<Measure>] 'w, [<Measure>] 'v, [<Measure>] 't> = 
        { Name : string
          Fermentables : Fermentable<'w> list
          Hops : hop<'w> list
          Adjuncts : adjunct<'w> list
          Yeast : yeast<'t> option
          Efficiency : float<percentage>
          BoilLength : float<minute>
          MashLength : float<minute>
          Volume : float<'v>
          Style : string 
          EstimatedOriginalGravity: float<sg>
          Bitterness:float<IBU>
          Colour:float<EBC>}
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    

    let CalculateGrainColour (grain:GrainAddition<_>) =
        GrainEBC grain.Weight grain.Grain.Colour

    let CalculateIBUs hopAddition sg vol =
        let utilisation = EstimateHopUtilisation sg (float hopAddition.Time)
        EstimateIBUs utilisation hopAddition.Hop.Alpha hopAddition.Weight vol

    let CalculateGravity volume efficiency fermentables =
        fermentables
        |> List.map (function Grain g -> g.Grain.Potential, g.Weight
                                | Adjunct a -> a.Adjunct.Potential, a.Weight)
        |> List.fold (fun acc f -> acc + EstimateGravityPoints (fst f) (snd f) volume efficiency) 0.0<gp>
        |> ToGravity

    let EstimateOriginalGravity recipe = 
        {recipe with EstimatedOriginalGravity = 
                        recipe.Fermentables
                        |> CalculateGravity recipe.Volume recipe.Efficiency}

    let CalculateColour recipe =
        recipe.Fermentables 
        |> List.choose (function Grain g -> Some g | _ -> None)
        |> List.map CalculateGrainColour
        |> TotalEBC recipe.Efficiency recipe.Volume


    let UpdateFermentables recipe fermentables = 
        { recipe with Fermentables = fermentables}

    let AddFermentable recipe fermentable = 
        fermentable :: recipe.Fermentables
        |> UpdateFermentables recipe
        |> EstimateOriginalGravity