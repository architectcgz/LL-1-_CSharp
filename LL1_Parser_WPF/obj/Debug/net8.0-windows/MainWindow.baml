   M S B A M L   `   `   `  ����  Exp2_WPF    Exp2_WPF.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  LL(1)��5   	   $	��600��6   $	��800��6$   .��5      � .+�5   
   `� $	��Top=�6   .�5      c� .@�5	      �� -   5        $��30��5	      $	��100��6%   .��5
      � $��30��6    $��80��6,   .+�5      ��    NewfileBtnImage#��[  6    $��20��6J   �� 5      $
��Wrap=�6$   $��添加文法��68   5      5      �� 5      �� 5      -   5        $��30��5      $	��100��6%   .��5      � $��30��6    $	��100��6,   .+�5      ��   RunBtnImage#��[ 6    $��20��6F   �� 5      $
��Wrap=�6$   $��分析字符串��68   5      5      �� 5      -   5        $��30��5      $	��100��6%   .��5      � $��30��6    $	��100��6,   .+�5      ��   LookBtnImage#��[ 6    $��20��6G   �� 5      $
��Wrap=�6$   $��查看分析表��68   5      5      5      5      � 5    
   $��0,40,0,0q�6    � RowDefinitions 5!      �� 5"      $
��Auto �6    �� 5#      $��* �6    �� 5$      $
��Auto �6    .+�5(      �� -   5         ��Name$ inputTextBox��5(      $��0��6,   $��10,10,10,396q�69   $��2��6O   &   System.Windows.Controls.DataGrid  5*      -   5          Name$ dataGrid��5*      $��1��6K   $��10,121,10,10q�6X      AutoGenerateColumns$ False��6n      HeadersVisibility$ Column=�6�      
IsReadOnly$
 True��6�   ��6)   �� *	StepInfos+   Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ 步骤��6)   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 86 6S   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 65   �� *	StepCount+6S     5-      $ 	分析栈��6)   $		 173 6X   
 66   �� *AnalysisStack+6X     5.      $ 剩余输入串��6)   $		 173 6[   
 68   �� *RemainingInput+6[     5/      $ 所用产生式��6)   $		 173 6[   
 68   �� *ProductionUsed+6[     50      $ 动作��6)   $		 173 6P   
 65   �� *Action+6P   52      55      �� 58   
   $��3��6   $��Bottom=�6!   .@�59      �� $��LL(1)��6   �� 5:      ��6   �� *	TimeDay+�� 5;      ��6   �� *TimeHM+�� 5<      -   5        $��statusTextBlock��5<      $��Ready��6/   5=      5>      5?      