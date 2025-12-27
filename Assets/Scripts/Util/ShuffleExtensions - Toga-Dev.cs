using System.Collections.Generic ;
using UnityEngine ;

// Taken from Toga-Dev on github:
public static class ShuffleExtension {

   public static void Shuffle<T> (this T[] array, int shuffleAccuracy) {
      for (var i = 0; i < shuffleAccuracy; i++) {
         var randomIndex = Random.Range (1, array.Length) ;
         (array [ randomIndex ], array [ 0 ]) = (array [ 0 ], array [ randomIndex ]);
      }
   }
   
   public static void Shuffle<T> (this List<T> list, int shuffleAccuracy) {
      for (var i = 0; i < shuffleAccuracy; i++) {
         var randomIndex = Random.Range (1, list.Count) ;
         (list [ randomIndex ], list [ 0 ]) = (list [ 0 ], list [ randomIndex ]);
      }
   }
}
