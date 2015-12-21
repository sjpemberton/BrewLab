namespace Models

open Units
open Calculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module Recipe = 
    type _recipe<[<Measure>] 'w, [<Measure>] 'v, [<Measure>] 't> = 
        { Name : string
          Ingredients : Ingredient list
          Yeast : Yeast<'t> option
          Efficiency : float<percentage>
          BoilLength : float<minute>
          MashLength : float<minute>
          Volume : float<'v>
          Style : string
          EstimatedOriginalGravity : float<sg>
          Bitterness : float<IBU>
          Colour : float<EBC> }
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    
    //Temporary conversion - Allign measures instead
    let gramsToKilos (g : float<g>) = float g * 1000.0 * 1.0<kg>
    
    let CalculateGrainColour fermentableAddition = 
        match fermentableAddition.Fermentable with
        | Grain(_p, colour, _) -> GrainEBC (gramsToKilos fermentableAddition.Weight) colour
        | _ -> 0.0<EBC>
    
    let CalculateIBUs hopAddition sg vol = 
        let (Hop(alpha, _)) = hopAddition.Hop
        let utilisation = EstimateHopUtilisation sg (float hopAddition.Time)
        EstimateIBUs utilisation alpha.Value hopAddition.Weight vol
    
    let CalculateGravity volume efficiency fermentables = 
        fermentables
        |> List.map (fun f -> 
               match f.Fermentable with
               | Grain(potential, _, _) -> potential, f.Weight |> gramsToKilos
               | Adjunct(potential, _) -> potential, f.Weight |> gramsToKilos)
        |> List.fold (fun acc f -> acc + EstimateGravityPoints (fst f) (snd f) volume efficiency) 0.0<gp>
        |> ToGravity
    
    let EstimateOriginalGravity recipe = 
        let gravity = 
            recipe.Ingredients
            |> List.choose (function 
                   | FermentableAddition f -> Some f
                   | _ -> None)
            |> CalculateGravity recipe.Volume recipe.Efficiency
        { recipe with EstimatedOriginalGravity = gravity }
    
    let CalculateColour recipe = 
        recipe.Ingredients
        |> List.choose (function 
               | FermentableAddition f -> Some f
               | _ -> None)
        |> List.map CalculateGrainColour
        |> TotalEBC recipe.Efficiency recipe.Volume

//    let UpdateFermentables recipe fermentables = 
//        { recipe with Ingredients = fermentables}
//
//    let AddFermentable recipe fermentable = 
//        fermentable :: recipe.Fermentables
//        |> UpdateFermentables recipe
//        |> EstimateOriginalGravity
