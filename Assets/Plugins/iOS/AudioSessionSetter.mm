#import "AVFoundation/AVFoundation.h"
 
@implementation AudioSessionSetter
 
extern "C"
{
     void _SetAudioAmbient()
    {
        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryAmbient error:nil];
        [[AVAudioSession sharedInstance] setActive:YES error:nil];
    }
 
    void _SetAudioPlayback(){
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:nil];
            [[AVAudioSession sharedInstance] setActive:YES error:nil];
    }
 
    void _SetAudioPlayAndRecord(){
            [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionMixWithOthers error:nil];
            [[AVAudioSession sharedInstance] setActive:YES error:nil];
    }
   
     bool _IsAmbientSet()
     {
          AVAudioSessionCategory cat = [[AVAudioSession sharedInstance] category];
          if(cat == AVAudioSessionCategoryAmbient){
              return true;
          }
          else return false;
     }
}
@end
 