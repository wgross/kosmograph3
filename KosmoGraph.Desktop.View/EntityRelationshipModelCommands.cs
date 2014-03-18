namespace KosmoGraph.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public static class EntityRelationshipModelCommands
    {
        public static RoutedCommand CreateEntity = new RoutedCommand();

        public static RoutedCommand CreateRelationshipWithEntities = new RoutedCommand();

        public static RoutedCommand CreateRelationshipWithEntity = new RoutedCommand();

        public static RoutedCommand CreateFacet = new RoutedCommand();

        public static RoutedCommand EditEntity = new RoutedCommand();

        public static RoutedCommand EditRelationship = new RoutedCommand();

        public static RoutedCommand EditTag = new RoutedCommand();

        public static RoutedCommand DeleteEntity = new RoutedCommand();

        public static RoutedCommand DeleteRelationship = new RoutedCommand();

        public static RoutedCommand DeleteFacet = new RoutedCommand();

        public static RoutedCommand CreateNewModel = new RoutedCommand();

        public static RoutedCommand SaveModelAs = new RoutedCommand();

        public static RoutedCommand SaveModel = new RoutedCommand();

        public static RoutedCommand CreateNewModelFromStore = new RoutedCommand();

        public static RoutedCommand LayoutModel = new RoutedCommand();
    }
}
