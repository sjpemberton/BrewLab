namespace Models

open Units

type hop = {Name:string; Weight:float<g>; Alpha:float<percentage>; }
type adjunct = {Name:string; Weight:float<g>; Alpha:float<percentage>; }


type Ingredient =
| Hop of hop
| Adjunct of adjunct