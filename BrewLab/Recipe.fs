namespace Models

open Units
open Calculations
open Conversions
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module Recipe = 
    type _recipe<[<Measure>] 'w, [<Measure>] 'v, [<Measure>] 't> = 
        { Name: string
          Ingredients: Ingredient list
          Yeast:Yeast<'t> option
          Efficiency:float<percentage>
          BoilLength:Time
          MashLength:Time
          Volume:float<'v>
          Style:string 
          EstimatedOriginalGravity: float<sg> 
          Bitterness:float<IBU>
          Colour:float<EBC>}
    
    type Recipe = 
        | Metric of _recipe<kg, L, degC>
        | Imperial of _recipe<lb, usGal, degF>
    
    //Temp conversion - Allign measures
    let gramsToKilos (g:float<g>) = float g * 1000.0 * 1.0<kg>

    let CalculateGrainColour (fermentableAddition:FermentableAddition) :float<EBC> = 
        match fermentableAddition with
        | (Grain (_,colour,_), weight) -> 
            let (Weight w) = weight //need value extractor
            GrainEBC (gramsToKilos w) colour
        | _ -> 0.0<EBC>

    let CalculateIBUs hopAddition sg vol =
        let (Weight w) = hopAddition.Weight   //need value extractor
        let (Time t) = hopAddition.Time       //need value extractor
        let (Hop (alpha,_)) = hopAddition.Hop //need value extractor
        let (Alpha a) = alpha                 //need value extractor

        let utilisation = EstimateHopUtilisation sg (float t)
        EstimateIBUs utilisation a w vol

    let CalculateGravity volume efficiency (fermentables:FermentableAddition list) =
        let potential (Potential p) = p //need value extractor
        let getWeight (Weight w) = w   //need value extractor
        fermentables
        |> List.map (function (Grain (potential,_,_), weight) -> potential, (getWeight weight)
                               | Adjunct (potential, _), weight -> potential, (getWeight weight))
        |> List.fold (fun acc f -> acc + EstimateGravityPoints (potential <|fst f) (gramsToKilos <| snd f) volume efficiency) 0.0<gp>
        |> ToGravity

    let EstimateOriginalGravity recipe =
        let gravity = recipe.Ingredients
                        |> List.choose (function FermentableAddition f -> Some f | _ -> None)
                        |> CalculateGravity recipe.Volume recipe.Efficiency
        {recipe with EstimatedOriginalGravity = gravity }

    let CalculateColour recipe =
        recipe.Ingredients
        |> List.choose (function FermentableAddition f -> Some f | _ -> None)
        |> List.map CalculateGrainColour
        |> TotalEBC recipe.Efficiency recipe.Volume


//    let UpdateFermentables recipe fermentables = 
//        { recipe with Ingredients = fermentables}
//
//    let AddFermentable recipe fermentable = 
//        fermentable :: recipe.Fermentables
//        |> UpdateFermentables recipe
//        |> EstimateOriginalGravity