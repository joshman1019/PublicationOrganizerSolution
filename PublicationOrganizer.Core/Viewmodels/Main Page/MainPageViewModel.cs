﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PublicationOrganizer.Core
{
    public class MainPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainPageViewModel()
        {
            // Performs the initial draw down from the database 
            BuildPublicationList(); 
        }

        /// <summary>
        /// Publicly accessible list of <see cref="Publication"/> that will be used by the UI to create a list. 
        /// </summary>
        public ObservableCollection<Publication> PublicationList { get; set; }

        /// <summary>
        /// Property that will contain the selected publication 
        /// </summary>
        public Publication SelectedPublication { get; set; }

        /// <summary>
        /// Property that contains a <see cref="Publication"/> that will be added to the database 
        /// </summary>
        public Publication NewWorkingPublication { get; set; }

        /// <summary>
        /// Publicly accessible boolean which determines the Add New Publication panel visibility
        /// </summary>
        public bool AddNewPublicationPanelVisibility { get; set; }

        /// <summary>
        /// Publicly accessible boolean which determines the Edit Publication panel visibility
        /// </summary>
        public bool EditPublicationPanelVisibility { get; set; }

        /// <summary>
        /// String that will be used to search for a publication title
        /// </summary>
        public string SearchString { get; set; }


        /// <summary>
        /// Method that populates the <see cref="PublicationList"/> properly with an <see cref="ObservableCollection{Publication}"/> drawn down from the database
        /// </summary>
        private void BuildPublicationList()
        {
            try
            {
                PublicationList = new READ_PublicationsFromDatabase().GetCollectionOfPublications();
            }
            catch (Exception ex)
            {
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("An error has occurred", ex.Message); 
            }
        }

        /// <summary>
        /// Command which shows the AddNewPublication panel upon execution
        /// </summary>
        public RelayCommand ShowAddNewPublication_COMMAND => new RelayCommand(() =>
        {
            NewWorkingPublication = new Publication();
            ShowAddPanel(); 
        });

        /// <summary>
        /// Command which hides the AddNewPublication panel upon execution
        /// </summary>
        public RelayCommand HideAddNewPublication_COMMAND => new RelayCommand(() =>
        {
            HideAddPanel(); 
        });

        /// <summary>
        /// Command which indexes a new publication into the database 
        /// </summary>
        public RelayCommand AddNewPublication_COMMAND => new RelayCommand(() =>
        {
            try
            {
                new CREATE_PublicationToDatabase().AddPublication(NewWorkingPublication);
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("Publication Added Successfully", 
                    $"The publication titled: {NewWorkingPublication.Title} has been added successfully");
                NewWorkingPublication = new Publication();
                HideAddPanel(); 
                BuildPublicationList(); 
            }
            catch (Exception ex)
            {
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("An error has occurred", ex.Message); 
            }
        });

        /// <summary>
        /// Command which deletes a selected publication from the database 
        /// </summary>
        public RelayCommand DeleteSelectedPublication_COMMAND => new RelayCommand(() =>
        {
            try
            {
                // Performs deletion 
                new DELETE_PublicationFromDatabase().PerformDelete(SelectedPublication.ID);
                // Prompts user of successful deletion
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("Deletion Successful", "the record was deleted successfully!");
                // Resets publication list
                BuildPublicationList(); 
            }
            catch (Exception ex)
            {
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("An error has occurred", ex.Message); 
            }
        }, SelectedPublication != null);

        /// <summary>
        /// Command that shows the publication edit panel
        /// </summary>
        public RelayCommand ShowPublicationEditPanel_COMMAND => new RelayCommand(() =>
        {
            ShowEditPanel();
        }, SelectedPublication != null);

        /// <summary>
        /// Command that closes the publication edit panel 
        /// </summary>
        public RelayCommand HidePublicationEditPanel_COMMAND => new RelayCommand(() =>
        {
            HideEditPanel(); 
        });

        /// <summary>
        /// Command that commits a publication edit to the database
        /// </summary>
        public RelayCommand CommitEditedPublication_COMMAND => new RelayCommand(() =>
        {
            try
            {
                new UPDATE_ExistingPublicationRecord().EditPublication(SelectedPublication);
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("Edit Recorded", "The record was updated successfully!");
                SelectedPublication = null;
                HideEditPanel(); 
                BuildPublicationList(); 
            }
            catch (Exception ex)
            {
                HideEditPanel(); 
                StaticViewmodelController.ApplicationViewModel.CreateMessageDialog("An error has occurred", ex.Message); 
                throw;
            }
        }, SelectedPublication != null);

        public RelayCommand PerformTitleSearch_COMMAND => new RelayCommand(() =>
        {
            PublicationList = new READ_PublicationsByTitle().GetCollectionOfPublications(SearchString); 
        }, !string.IsNullOrEmpty(SearchString));

        public RelayCommand ResetSearch_COMMAND => new RelayCommand(() =>
        {
            PublicationList = new READ_PublicationsFromDatabase().GetCollectionOfPublications();
            SearchString = string.Empty; 
        });

        private void HideEditPanel()
        {
            EditPublicationPanelVisibility = false; 
        }
        private void ShowEditPanel()
        {
            EditPublicationPanelVisibility = true; 
        }
        private void HideAddPanel()
        {
            AddNewPublicationPanelVisibility = false; 
        }
        private void ShowAddPanel() 
        {
            AddNewPublicationPanelVisibility = true; 
        }
    }
}
