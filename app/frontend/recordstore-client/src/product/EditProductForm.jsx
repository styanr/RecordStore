import React, { useState, useEffect } from 'react';
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  FormControl,
  FormLabel,
  Input,
  Textarea,
  Button,
  useToast,
  Select,
  Box,
} from '@chakra-ui/react';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteList,
  AutoCompleteItem,
  AutoCompleteTag,
} from '@choc-ui/chakra-autocomplete';

// should use useDebounce in this, couldn't get it to work
// import { useDebounce } from '@uidotdev/usehooks';
import productService from '../home/ProductService';
import genreService from '../home/GenreService';
import artistService from './ArtistService';
import formatService from '../home/FormatService';

const EditProductForm = ({ isOpen, onClose, product, onSubmit }) => {
  const [formData, setFormData] = useState({
    // title: product ? product.title : '',
    description: product ? product.description : '',
    price: product ? product.price : '',
    format: product ? (product.format || {}).name : '', // should be format.name (not format
    releaseDate: product ? product.releaseDate : '',
    // artists: product ? product.artists || [] : [],
    // recordId: product ? product.recordId : '',
    // genres: product ? (product.genres || []).map((genre) => genre.name) : [],
  });

  const [id, setId] = useState(product ? product.id : '');

  //   const [searchArtistTerm, setSearchArtistTerm] = useState('');
  //   const [artists, setArtists] = useState([]);
  //   const [searchGenreTerm, setSearchGenreTerm] = useState('');
  //   const [genres, setGenres] = useState([]);
  const [searchFormatTerm, setSearchFormatTerm] = useState('');
  const [formats, setFormats] = useState([]);

  useEffect(() => {
    console.log(formData);
  }, [formData]);

  //   useEffect(() => {
  //     const fetchArtists = async () => {
  //       try {
  //         const response = await artistService.searchArtists(searchArtistTerm);
  //         setArtists(response);
  //       } catch (error) {
  //         console.error('Error fetching artists:', error);
  //       }
  //     };
  //     if (searchArtistTerm) {
  //       fetchArtists();
  //     }
  //   }, [searchArtistTerm]);

  //   useEffect(() => {
  //     const fetchGenres = async () => {
  //       try {
  //         const response = await genreService.getGenres(searchGenreTerm);
  //         setGenres(response);
  //       } catch (error) {
  //         console.error('Error fetching genres:', error);
  //       }
  //     };
  //     if (searchGenreTerm) {
  //       fetchGenres();
  //     }
  //   }, [searchGenreTerm]);

  useEffect(() => {
    const fetchFormats = async () => {
      try {
        const response = await formatService.searchFormats(searchFormatTerm);
        setFormats(response);
      } catch (error) {
        console.error('Error fetching formats:', error);
      }
    };
    if (searchFormatTerm) {
      fetchFormats();
    }
  }, [searchFormatTerm]);

  useEffect(() => {
    if (product) {
      setFormData({
        // title: product ? product.title : '',
        description: product ? product.description : '',
        price: product ? product.price : '',
        format: product ? (product.format || {}).name : '', // should be format.name (not format
        // releaseDate: product ? product.releaseDate : '',
        // artists: product ? product.artists || [] : [],
        // recordId: product ? product.recordId : '',
        // genres: product
        //   ? (product.genres || []).map((genre) => genre.name)
        //   : [],
      });

      setId(product.id);
    }
    setSearchFormatTerm(product ? (product.format || {}).name : '');
  }, [product]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  //   const handleArtistChange = (selectedArtists) => {
  //     setFormData({ ...formData, artists: selectedArtists });
  //   };

  //   const handleGenreChange = (vals) => {
  //     setFormData({ ...formData, genres: vals });
  //   };

  const handleFormatChange = (name) => {
    setFormData({ ...formData, format: name });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await onSubmit(id, formData);
  };
  return (
    <Modal isOpen={isOpen} onClose={onClose} size='xl'>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Редагувати продукт</ModalHeader>
        <ModalCloseButton />
        <form onSubmit={handleSubmit}>
          <ModalBody>
            {/* <FormControl id='title' mb={4}>
              <FormLabel>Назва</FormLabel>
              <Input
                name='title'
                value={formData.title}
                onChange={handleChange}
              />
            </FormControl> */}
            <FormControl id='description' mb={4}>
              <FormLabel>Опис</FormLabel>
              <Textarea
                name='description'
                value={formData.description}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='price' mb={4}>
              <FormLabel>Ціна (грн)</FormLabel>
              <Input
                name='price'
                type='number'
                value={formData.price}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='formats' mb={4}>
              <FormLabel>Формат</FormLabel>
              <AutoComplete openOnFocus mb={4}>
                <AutoCompleteInput
                  placeholder='Формат'
                  onChange={(e) => {
                    setSearchFormatTerm(e.target.value);
                  }}
                  value={searchFormatTerm}
                  mb={4}
                />
                <AutoCompleteList>
                  {formats.map((option, index) => (
                    <AutoCompleteItem
                      key={index}
                      onClick={() => {
                        setSearchFormatTerm(option.name);
                        handleFormatChange(option.name);
                      }}
                      value={option.name}
                    >
                      {option.name}
                    </AutoCompleteItem>
                  ))}
                </AutoCompleteList>
              </AutoComplete>
            </FormControl>
            {/*
              <AutoComplete
                openOnFocus
                mb={4}
                multiple
                creatable
                onChange={handleArtistChange}
                defaultValues={formData.artists.map((artist) => artist.name)}
              >
                <AutoCompleteInput
                  variant='filled'
                  onChange={(e) => setSearchArtistTerm(e.target.value)}
                >
                  {({ tags }) =>
                    tags.map((tag, tid) => (
                      <AutoCompleteTag
                        key={tid}
                        label={tag.label}
                        onRemove={tag.onRemove}
                        backgroundColor='gray.300'
                      />
                    ))
                  }
                </AutoCompleteInput>
                <AutoCompleteList>
                  {artists.map((artist) => (
                    <AutoCompleteItem key={artist.name} value={artist.name}>
                      {artist.name}
                    </AutoCompleteItem>
                  ))}
                </AutoCompleteList>
              </AutoComplete>
            </FormControl> */}
            {/* <FormControl id='genres' mb={4}>
              <FormLabel>Жанри</FormLabel>
              <Box position='relative'>
                <AutoComplete
                  openOnFocus
                  mb={4}
                  multiple
                  creatable
                  onChange={handleGenreChange}
                  defaultValues={formData.genres}
                >
                  <AutoCompleteInput
                    variant='filled'
                    onChange={(e) => setSearchGenreTerm(e.target.value)}
                  >
                    {({ tags }) =>
                      tags.map((tag, tid) => (
                        <AutoCompleteTag
                          key={tid}
                          label={tag.label}
                          onRemove={tag.onRemove}
                          backgroundColor='gray.300'
                        />
                      ))
                    }
                  </AutoCompleteInput>
                  <AutoCompleteList>
                    {genres.map((genre) => (
                      <AutoCompleteItem key={genre.name} value={genre.name}>
                        {genre.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
              </Box>
            </FormControl> */}
          </ModalBody>
          <ModalFooter>
            <Button colorScheme='blue' mr={3} type='submit'>
              Оновити
            </Button>
            <Button variant='ghost' onClick={onClose}>
              Скасувати
            </Button>
          </ModalFooter>
        </form>
      </ModalContent>
    </Modal>
  );
};

export default EditProductForm;
