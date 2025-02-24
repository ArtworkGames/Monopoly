using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingPlace : MonoBehaviour
{
	public Action<BuildingPlace> OnObstacleRemove;
	public Action<BuildingPlace> OnConstructionComplete;
	public Action<BuildingPlace> OnClick;
	public Action<BuildingPlace> OnSelect;
	public Action<BuildingPlace> OnUnselect;

	public District district;

	[Space]
	public Transform pivot;
	public ConstructionObstacle constructionObstacle;
	public Construction construction;
	public Building building;

	[Space]
	public float cameraYaw;
	public float cameraPitch;
	public float cameraZoom;

	[HideInInspector] public BuildingData data;

	private Coroutine constructionCoroutine;
	private bool isConstructionComplete;

	private Tween scaleTween;

	private void Start()
	{
	}

	public void Init(BuildingData buildingData)
	{
		data = buildingData;

		constructionObstacle.OnClick += OnConstructionObstacleClick;
		construction.OnClick += OnConstructionClick;
		building.OnClick += OnBuildingClick;

		//selection.SetColor(DistrictData.GetColor(district.color));

		constructionObstacle.gameObject.SetActive(false);
		construction.gameObject.SetActive(false);
		building.gameObject.SetActive(false);

		if ((data.district.isActive) &&
			(data.constructionState == ConstructionState.NeedToClean))
		{
			constructionObstacle.gameObject.SetActive(true);
			constructionObstacle.Show();
		}
		if (!data.district.isActive)
		{
			data.district.OnActivated += OnDistrictActivated;
		}

		//for (int i = 0; i < additionalAnimations.Length; i++)
		//{
		//	additionalAnimations[i].SetActive(false);
		//}
	}

	private void OnDestroy()
	{
		if (constructionCoroutine != null)
		{
			StopCoroutine(constructionCoroutine);
		}
		//if (data != null)
		//{
		//	data.district.OnActivated -= OnDistrictActivated;
		//}
	}

	private void OnDistrictActivated(DistrictData districtData)
	{
		data.district.OnActivated -= OnDistrictActivated;

		if (data.constructionState == ConstructionState.NeedToClean)
		{
			constructionObstacle.gameObject.SetActive(true);
			constructionObstacle.Show();
		}
	}

	public void RemoveObstacle()
	{
		constructionObstacle.Hide();
		OnObstacleRemove?.Invoke(this);
	}

	public void ConstructBuilding()
	{
		construction.gameObject.SetActive(true);
		construction.Show();
		constructionCoroutine = StartCoroutine(ConstructionCoroutine());
	}

	private IEnumerator ConstructionCoroutine()
	{
		yield return new WaitForSeconds(data.constructionDuration);
		CompleteConstruction();
	}

	public void SkipConstruction()
	{
		if (constructionCoroutine != null)
		{
			StopCoroutine(constructionCoroutine);
		}
		CompleteConstruction();
	}

	private void CompleteConstruction()
	{
		if (!isConstructionComplete)
		{
			isConstructionComplete = true;

			construction.Hide();
			building.gameObject.SetActive(true);
			building.Show();

			//for (int i = 0; i < additionalAnimations.Length; i++)
			//{
			//	additionalAnimations[i].SetActive(true);
			//}

			OnConstructionComplete?.Invoke(this);
		}
	}

	private void OnConstructionObstacleClick(ClickableObject clickableObject)
	{
		AnimateClick();
		OnClick?.Invoke(this);
	}

	private void OnConstructionClick(ClickableObject clickableObject)
	{
		AnimateClick();
		OnClick?.Invoke(this);
	}

	private void OnBuildingClick(ClickableObject clickableObject)
	{
		AnimateClick();
		OnClick?.Invoke(this);
	}

	private void AnimateClick()
	{
		scaleTween?.Kill();
		scaleTween = pivot.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.1f)
			.SetEase(Ease.OutQuad)
			.OnComplete(() =>
			{
				scaleTween = pivot.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f)
					.SetEase(Ease.OutElastic);
			});
	}

	public void Select()
	{
		AnimateClick();
		//selection.Select();
		OnSelect?.Invoke(this);
	}

	public void Unselect()
	{
		//selection.Unselect();
		OnUnselect?.Invoke(this);
	}
}
