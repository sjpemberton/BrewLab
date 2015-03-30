namespace Models

open Units

type Recipe = {
    Grain: grain list
    Hops: hop list
    Adjuncts: adjunct list
    Yeast: yeast
    Name: string
    Efficincy: float<percentage>
    BoilLength:float//In minutes
    MashLength:float

}